using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.ObtainPvpcLib.Services;

public class ObtainPvpCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<ObtainPvpCronBackgroundService> Logger;

    private Settings.ObtainPvpcSettings Options => (Settings.ObtainPvpcSettings)Config;

    public ObtainPvpCronBackgroundService(
        Settings.ObtainPvpcSettings config
        , IServiceProvider serviceProvider
        , ILogger<ObtainPvpCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        // TODO             Issue #7
        string AppName = ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        DateTime ForDate = DateTimeOffset.UtcNow.AddDays(1).Date;

        await ObtainPvpcForDateAsync(ForDate, stoppingToken);

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    public async Task ObtainPvpcForDateAsync(DateTime forDate, CancellationToken stoppingToken)
    {
        Logger.LogInformation("Obtaining PVPC for the day {ForDate}", forDate.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat));

        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        HttpClient Client = new();

        string UrlString = string.Format(Options.DataUrlTemplate, forDate);
        Logger.LogInformation("From {UrlString}", UrlString);

        try
        {
            Rootobject? Response = await Client.GetFromJsonAsync<Rootobject>(UrlString, stoppingToken);

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
    }

    private async Task<int?> ProcessPricesAsync(CoreLib.Entities.Pvpc[]? NewEntities, CancellationToken stoppingToken)
    {
        if (!(NewEntities?.Any() ?? false))
        {
            Logger.LogInformation("No entities obtained");
            return null;
        }

        var Prices = new List<CoreLib.Entities.PvpcBase>(24);

        IEnumerable<long> DateTimes = NewEntities.Select(x => x.AtDateTimeOffset.ToUnixTimeSeconds());
        long MinDateTime = DateTimes.Min();
        long MaxDateTime = DateTimes.Max();

        InfrastructureLib.DbContexts.DbCxt dbCxt = ServiceProvider.GetRequiredService<InfrastructureLib.DbContexts.DbCxt>();

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
                System.Text.Json.JsonSerializer.Serialize<IEnumerable<CoreLib.Entities.Pvpc>>(Prices.Cast<CoreLib.Entities.Pvpc>()));
            _ = await dbCxt.Outbox.AddAsync(OutboxMessage, stoppingToken);

            _ = await dbCxt.SaveChangesAsync(stoppingToken);

            Logger.LogInformation("Obtained {NewEntities} entities", NewEntities.Length);

            return Prices.Count;
        }
        else
        {
            Logger.LogInformation("No changes");

            return 0;
        }
    }
}
