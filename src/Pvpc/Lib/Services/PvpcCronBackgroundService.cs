using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.Pvpc.Lib.Services;

public sealed class PvpcCronBackgroundService : Libs.BackgroundServices.Cron
{
    // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
    private static readonly HttpClient httpClient = new();

    private readonly ILogger<PvpcCronBackgroundService> Logger;
    private Settings.PvpcSettings Settings => (Settings.PvpcSettings)Config;

    public PvpcCronBackgroundService(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        Logger = ServiceProvider.GetRequiredService<ILogger<PvpcCronBackgroundService>>();

        Config = ServiceProvider.GetRequiredService<IConfiguration>()
            .GetSection(nameof(Lib.Settings.PvpcSettings)).Get<Settings.PvpcSettings>()!;
    }

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            System.Diagnostics.Debugger.Break();

        DateTime ForDate = DateTimeOffset.UtcNow.AddDays(1).Date;

        _ = GetPvpcFromReeForDateAsync(ForDate, stoppingToken);

        await Task.CompletedTask;
    }

    public async Task GetPvpcFromReeForDateAsync(DateTime forDate, CancellationToken stoppingToken)
    {
        string? AppName = GetType().FullName;

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("Obtaining PVPC for the day {ForDate}", forDate.ToString(Libs.Core.Constants.Formats.YearMonthDay));

        string UrlString = string.Format(Settings.DataUrlTemplate, forDate);
        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("From {UrlString}", UrlString);

        try
        {
            Rootobject? Response = await httpClient.GetFromJsonAsync<Rootobject>(UrlString, stoppingToken);

            Included? PvpcIncluded = Response?.Included?.FirstOrDefault(x => x.Id == Settings.PvpcId);

            var NewEntities = PvpcIncluded?.Attributes?.Values?
                .Select(x => new Libs.Core.Entities.Pvpc(x.Datetime.GetValueOrDefault(), (decimal)x.Val.GetValueOrDefault()))
                .ToHashSet();

            int? HowManyPricesObtained = await ProcessPricesAsync(NewEntities, stoppingToken);
        }
        catch (HttpRequestException e) when (System.Net.HttpStatusCode.BadGateway == e.StatusCode && Logger.LogAndHandle(e, "'{WebUrl}' not yet published", UrlString)) { }
        catch (TaskCanceledException e) when (e.InnerException is TimeoutException && Logger.LogAndHandle(e, "Request to '{WebUrl}' timeout", UrlString)) { }
        catch (TaskCanceledException e) when (Logger.LogAndHandle(e, "Task request to '{WebUrl}' cancelled", UrlString)) { }
        catch (Exception e) when (Logger.LogAndHandle(e, "Request to '{WebUrl}' failed", UrlString)) { }

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("End {ApplicationName}", AppName);
    }

    private async Task<int?> ProcessPricesAsync(HashSet<Libs.Core.Entities.Pvpc>? NewEntities, CancellationToken stoppingToken)
    {
        if (NewEntities == null || NewEntities.Count == 0)
        {
            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("No entities obtained");
            return null;
        }

        Libs.Infrastructure.DbContexts.DbCxt dbCxt = ServiceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCxt>();

        DateTimeOffset MinDateTimeOffset = NewEntities.Min(x => x.AtDateTimeOffset);
        DateTimeOffset MaxDateTimeOffset = NewEntities.Max(x => x.AtDateTimeOffset);
        Libs.Core.Entities.Pvpc[] ExistingPvpcs = await dbCxt.Pvpcs
            .Where(p => p.AtDateTimeOffset >= MinDateTimeOffset && p.AtDateTimeOffset <= MaxDateTimeOffset)
            .ToArrayAsync(stoppingToken);

        List<Libs.Core.Entities.PvpcBase> Prices = new(24);
        foreach ((Libs.Core.Entities.Pvpc NewEntity, Libs.Core.Entities.Pvpc ExistingEntity) in
            from Libs.Core.Entities.Pvpc NewEntity in NewEntities
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
            Libs.Core.Entities.Outbox OutboxMessage = new(
                Libs.Core.Enums.SubscriptionName.electricidad,
                System.Text.Json.JsonSerializer.Serialize(Prices.Cast<Libs.Core.Entities.Pvpc>().ToArray()));
            _ = await dbCxt.Outbox.AddAsync(OutboxMessage, stoppingToken);

            _ = await dbCxt.SaveChangesAsync(stoppingToken);

            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("Obtained {NewEntities} entities", NewEntities.Count);

            return Prices.Count;
        }

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("No changes");

        return 0;
    }

    internal static bool IsTimeToCharge(
        Libs.Core.Entities.Pvpc[] pvpcs,
        DateTimeOffset timeToCheckDateTimeOffset,
        Settings.TuyaManagerSettings tuyaManagerSettings,
        ILogger logger)
    {
        if (pvpcs.Length == 0)
            return false;

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Checking if it's time to charge at {timeToCheckDateTimeOffset}", timeToCheckDateTimeOffset);
        Libs.Core.Entities.Pvpc CurrentHourPrice = pvpcs.OrderBy(x => x.AtDateTimeOffset).Last(x => x.AtDateTimeOffset <= timeToCheckDateTimeOffset);
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Current hour price: {CurrentHourPrice}", CurrentHourPrice);

        if (CurrentHourPrice.KWhPriceInEuros < tuyaManagerSettings.AllowChargeWhenKWhPriceInEurosIsBelowThan)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Allowed to charge because current hour price {CurrentHourPrice} is below {AllowChargeWhenKWhPriceInEurosIsBelowThan}", CurrentHourPrice.KWhPriceInEuros, tuyaManagerSettings.AllowChargeWhenKWhPriceInEurosIsBelowThan);
            return true;
        }

        decimal MaxOfTheCheapestHours = pvpcs.OrderBy(x => x.KWhPriceInEuros).Take(tuyaManagerSettings.ChargingHoursPerDay).Max(x => x.KWhPriceInEuros);
        if (CurrentHourPrice.KWhPriceInEuros < MaxOfTheCheapestHours)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Allowed to charge because current hour price {CurrentHourPrice} is below the max of the {ChargingHoursPerDay} cheapest hours {MaxOfTheCheapestHours}", CurrentHourPrice.KWhPriceInEuros, tuyaManagerSettings.ChargingHoursPerDay, MaxOfTheCheapestHours);
            return true;
        }

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Not allowed to charge because current hour price {CurrentHourPrice} is above {AllowChargeWhenKWhPriceInEurosIsBelowThan} and above the max of the {ChargingHoursPerDay} cheapest hours {MaxOfTheCheapestHours}", CurrentHourPrice.KWhPriceInEuros, tuyaManagerSettings.AllowChargeWhenKWhPriceInEurosIsBelowThan, tuyaManagerSettings.ChargingHoursPerDay, MaxOfTheCheapestHours);
        return false;
    }

    public override void Dispose()
    {
        httpClient.Dispose();

        base.Dispose();
    }
}
