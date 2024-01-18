using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seedysoft.CoreLib.Entities;
using Seedysoft.PvpcLib.Extensions;
using Seedysoft.PvpcLib.Settings;
using Seedysoft.UtilsLib.Extensions;
using System.Diagnostics;

namespace Seedysoft.PvpcLib.Services;

public sealed class TuyaManagerCronBackgroundService(
    TuyaManagerSettings config
    , InfrastructureLib.DbContexts.DbCxt dbCxt
    , ILogger<TuyaManagerCronBackgroundService> logger) : CronBackgroundServiceLib.CronBackgroundService(config)
{
    private readonly InfrastructureLib.DbContexts.DbCxt DbCxt = dbCxt;
    private readonly ILogger<TuyaManagerCronBackgroundService> Logger = logger;

    private TuyaManagerSettings Options => (TuyaManagerSettings)Config;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // Execute on init
        await DoWorkAsync(cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        string? AppName = GetType().FullName;

        Logger.LogDebug("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            TuyaDevice[] Devices = await DbCxt.TuyaDevices.AsNoTracking().ToArrayAsync(cancellationToken: stoppingToken);

            DateTimeOffset timeToCheckDateTimeOffset = DateTimeOffset.Now;
            DateTimeOffset dateToQueryDateTimeOffset = timeToCheckDateTimeOffset.Subtract(timeToCheckDateTimeOffset.TimeOfDay);

            Pvpc[] PricesForDayPvpcs = await DbCxt.Pvpcs.AsNoTracking()
                .Where(x => x.AtDateTimeOffset >= dateToQueryDateTimeOffset)
                .Where(x => x.AtDateTimeOffset < dateToQueryDateTimeOffset.AddDays(1))
                .ToArrayAsync(cancellationToken: stoppingToken);
            bool IsTimeToCharge = PvpcCronBackgroundService.IsTimeToCharge(PricesForDayPvpcs, timeToCheckDateTimeOffset, allowWhenKWhPriceInEurosBelow: 0.07M);

            for (int i = 0; i < Devices.Length; i++)
            {
                TuyaDevice tuyaDevice = Devices[i];
                var tuyaDeviceBase = tuyaDevice.ToTuyaDeviceBase();
                //_ = tuyaDeviceBase.GetStatus();

                object? TurnResult = IsTimeToCharge ? tuyaDeviceBase.TurnOn() : tuyaDeviceBase.TurnOff();
                if (TurnResult is Dictionary<string, object> dict)
                {
                    Debug.WriteLine($"Turn {IsTimeToCharge} results:");
                    foreach (KeyValuePair<string, object> kvp in dict)
                        Debug.WriteLine($"\t'{kvp.Key}'='{kvp.Value}'");
                }
                else
                {
                    Debug.WriteLine($"Turn {IsTimeToCharge} result: '{TurnResult}'");
                }
            }
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogDebug("End {ApplicationName}", AppName);
    }
}
