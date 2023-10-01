using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.ObtainPvpcLib;

public static class Main
{
    public static async Task<int?> ObtainPricesAsync(
        DbContexts.DbCxt dbCxt,
        Settings.ObtainPvpcSettings options,
        ILogger logger,
        DateTime forDate,
        CancellationToken stoppingToken)
    {
        logger.LogInformation("Obtaining PVPC for the day {forDate}", forDate.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat));

        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        HttpClient Client = new();

        string UrlString = string.Format(options.DataUrlTemplate, forDate);
        logger.LogInformation("From {UrlString}", UrlString);

        try
        {
            Rootobject? Response = await Client.GetFromJsonAsync<Rootobject>(UrlString, stoppingToken);

            Included? PvpcIncluded = Response?.included?.FirstOrDefault(x => x.id == options.PvpcId);

            CoreLib.Entities.Pvpc[]? NewEntities = PvpcIncluded?.attributes?.values?
                .Select(x => new CoreLib.Entities.Pvpc(x.datetime.GetValueOrDefault(), (decimal)x.value.GetValueOrDefault()))
                .ToArray();

            if (!(NewEntities?.Any() ?? false))
            {
                logger.LogInformation("No entities obtained");
                return null;
            }

            var Prices = new List<CoreLib.Entities.PvpcBase>(24);

            IEnumerable<long> DateTimes = NewEntities.Select(x => x.AtDateTimeOffset.ToUnixTimeSeconds());
            long MinDateTime = DateTimes.Min();
            long MaxDateTime = DateTimes.Max();

            CoreLib.Entities.PvpcView[] ExistingPvpcs =
                await dbCxt.PvpcsView
                .Where(p => p.AtDateTimeUnix >= MinDateTime && p.AtDateTimeUnix <= MaxDateTime)
                .ToArrayAsync(stoppingToken);

            foreach ((CoreLib.Entities.Pvpc NewEntity, CoreLib.Entities.PvpcView ExistingEntity) in
                from CoreLib.Entities.Pvpc NewEntity in NewEntities
                let ExistingEntity = ExistingPvpcs.FirstOrDefault(x => x.AtDateTimeOffset == NewEntity.AtDateTimeOffset)
                select (NewEntity, ExistingEntity))
            {
                if (ExistingEntity == null)
                {
                    Prices.Add(NewEntity);
                    _ = dbCxt.Pvpcs.Add(NewEntity);
                }
                else
                {
                    Prices.Add(ExistingEntity);
                    ExistingEntity.MWhPriceInEuros = NewEntity.MWhPriceInEuros;
                    if (dbCxt.Entry(ExistingEntity).State == EntityState.Modified)
                        _ = dbCxt.Update(ExistingEntity);
                }
            }

            if (dbCxt.ChangeTracker.HasChanges())
            {
                var OutboxMessage = new CoreLib.Entities.Outbox(
                    CoreLib.Enums.SubscriptionName.electricidad,
                    System.Text.Json.JsonSerializer.Serialize<IEnumerable<CoreLib.Entities.PvpcBase>>(Prices));
                _ = await dbCxt.Outbox.AddAsync(OutboxMessage, stoppingToken);

                _ = await dbCxt.SaveChangesAsync(stoppingToken);

                logger.LogInformation("Obtained {NewEntities} entities", NewEntities.Length);

                return Prices.Count;
            }
            else
            {
                logger.LogInformation("No changes");

                return 0;
            }
        }
        catch (HttpRequestException e) when (System.Net.HttpStatusCode.BadGateway == e.StatusCode && logger.LogAndHandle(e, "'{WebUrl}' not yet published", UrlString)) { }
        catch (TaskCanceledException e) when (e.InnerException is TimeoutException && logger.LogAndHandle(e, "Request to '{WebUrl}' timeout", UrlString)) { }
        catch (TaskCanceledException e) when (logger.LogAndHandle(e, "Task request to '{WebUrl}' cancelled", UrlString)) { }
        catch (Exception e) when (logger.LogAndHandle(e, "Request to '{WebUrl}' failed", UrlString)) { }

        return null;
    }
}
