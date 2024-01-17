using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seedysoft.PvpcLib.Settings;
using Seedysoft.UtilsLib.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.PvpcLib.Services;

public sealed class PvpcCronBackgroundService(
    PvpcSettings config
    , InfrastructureLib.DbContexts.DbCxt dbCxt
    , ILogger<PvpcCronBackgroundService> logger) : CronBackgroundServiceLib.CronBackgroundService(config)
{
    // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
    private static readonly HttpClient client = new();

    private readonly InfrastructureLib.DbContexts.DbCxt DbCxt = dbCxt;
    private readonly ILogger<PvpcCronBackgroundService> Logger = logger;

    private PvpcSettings Options => (PvpcSettings)Config;

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        DateTime ForDate = DateTimeOffset.UtcNow.AddDays(1).Date;

        await PvpcForDateAsync(ForDate, stoppingToken);
    }

    public async Task PvpcForDateAsync(DateTime forDate, CancellationToken stoppingToken)
    {
        string? AppName = GetType().FullName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        Logger.LogInformation("Obtaining PVPC for the day {ForDate}", forDate.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat));

        string UrlString = string.Format(Options.DataUrlTemplate, forDate);
        Logger.LogInformation("From {UrlString}", UrlString);

        try
        {
            Rootobject? Response = await client.GetFromJsonAsync<Rootobject>(UrlString, stoppingToken);

            Included? PvpcIncluded = Response?.included?.FirstOrDefault(x => x.id == Options.PvpcId);

            CoreLib.Entities.Pvpc[]? NewEntities = PvpcIncluded?.attributes?.values?
                .Select(x => new CoreLib.Entities.Pvpc(x.datetime.GetValueOrDefault(), (decimal)x.value.GetValueOrDefault()))
                .ToArray();

            int? HowManyPricesObtained = await ProcessPricesAsync(NewEntities, stoppingToken);
        }
        catch (HttpRequestException e) when (System.Net.HttpStatusCode.BadGateway == e.StatusCode && Logger.LogAndHandle(e, "'{WebUrl}' not yet published", UrlString)) { }
        catch (TaskCanceledException e) when (e.InnerException is TimeoutException && Logger.LogAndHandle(e, "Request to '{WebUrl}' timeout", UrlString)) { }
        catch (TaskCanceledException e) when (Logger.LogAndHandle(e, "Task request to '{WebUrl}' cancelled", UrlString)) { }
        catch (Exception e) when (Logger.LogAndHandle(e, "Request to '{WebUrl}' failed", UrlString)) { }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    private async Task<int?> ProcessPricesAsync(CoreLib.Entities.Pvpc[]? NewEntities, CancellationToken stoppingToken)
    {
        if (!(NewEntities?.Length > 0))
        {
            Logger.LogInformation("No entities obtained");
            return null;
        }

        List<CoreLib.Entities.PvpcBase> Prices = new(24);

        CoreLib.Entities.Pvpc[] ExistingPvpcs =
            await DbCxt.Pvpcs
            .Where(p => p.AtDateTimeOffset >= NewEntities.Min(x => x.AtDateTimeOffset) && p.AtDateTimeOffset <= NewEntities.Max(x => x.AtDateTimeOffset))
            .ToArrayAsync(stoppingToken);

        foreach ((CoreLib.Entities.Pvpc NewEntity, CoreLib.Entities.Pvpc ExistingEntity) in
            from CoreLib.Entities.Pvpc NewEntity in NewEntities
            let ExistingEntity = ExistingPvpcs.FirstOrDefault(x => x.AtDateTimeOffset == NewEntity.AtDateTimeOffset)
            select (NewEntity, ExistingEntity))
        {
            if (ExistingEntity == null)
            {
                Prices.Add(NewEntity);
                _ = DbCxt.Pvpcs.Add(NewEntity);
            }
            else
            {
                Prices.Add(ExistingEntity);
                ExistingEntity.MWhPriceInEuros = NewEntity.MWhPriceInEuros;
                if (DbCxt.Entry(ExistingEntity).State == EntityState.Modified)
                    _ = DbCxt.Update(ExistingEntity);
            }
        }

        if (DbCxt.ChangeTracker.HasChanges())
        {
            CoreLib.Entities.Outbox OutboxMessage = new(
                CoreLib.Enums.SubscriptionName.electricidad,
                System.Text.Json.JsonSerializer.Serialize(Prices.Cast<CoreLib.Entities.Pvpc>().ToArray()));
            _ = await DbCxt.Outbox.AddAsync(OutboxMessage, stoppingToken);

            _ = await DbCxt.SaveChangesAsync(stoppingToken);

            Logger.LogInformation("Obtained {NewEntities} entities", NewEntities.Length);

            return Prices.Count;
        }
        else
        {
            Logger.LogInformation("No changes");

            return 0;
        }
    }

    internal static bool IsTimeToCharge(
        CoreLib.Entities.Pvpc[] pvpcs
        , DateTimeOffset timeToCheckDateTimeOffset
        , decimal allowWhenKWhPriceInEurosBelow = decimal.Zero)
    {
        if (pvpcs.Length == 0)
            return false;

        CoreLib.Entities.Pvpc? CurrentHourPrice = pvpcs.LastOrDefault(x => x.AtDateTimeOffset <= timeToCheckDateTimeOffset);

        const int ChargingHoursPerDay = 4;

        return CurrentHourPrice != null &&
            (CurrentHourPrice.KWhPriceInEuros < allowWhenKWhPriceInEurosBelow ||
            CurrentHourPrice.KWhPriceInEuros < pvpcs.OrderBy(x => x.KWhPriceInEuros).Take(ChargingHoursPerDay).Max(x => x.KWhPriceInEuros));
    }

    public override void Dispose()
    {
        client.Dispose();

        base.Dispose();
    }
}
