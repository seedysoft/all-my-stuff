using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seedysoft.CoreLib.Entities;
using Seedysoft.PvpcLib.Extensions;
using Seedysoft.PvpcLib.Settings;
using Seedysoft.UtilsLib.Extensions;

namespace Seedysoft.PvpcLib.Services;

public sealed class TuyaManagerCronBackgroundService(
    TuyaManagerSettings config
    , InfrastructureLib.DbContexts.DbCxt dbCxt
    , ILogger<TuyaManagerCronBackgroundService> logger) : CronBackgroundServiceLib.CronBackgroundService(config)
{
    private TuyaManagerSettings Options => (TuyaManagerSettings)Config;

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        string? AppName = GetType().FullName;

        logger.LogDebug("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            DateTimeOffset timeToCheckDateTimeOffset = DateTimeOffset.UtcNow;
            timeToCheckDateTimeOffset = timeToCheckDateTimeOffset.AddMinutes(timeToCheckDateTimeOffset.TimeOfDay.Minutes);
            DateTime timeToCheckNowDateTime = timeToCheckDateTimeOffset.Date;
            double min = timeToCheckNowDateTime.Subtract(DateTimeOffset.UnixEpoch.Date).TotalSeconds;
            double max = timeToCheckNowDateTime.AddDays(1).Subtract(DateTimeOffset.UnixEpoch.Date).TotalSeconds;

            PvpcView[] todayPvpcViews = await dbCxt.PvpcsView
                .Where(x => x.AtDateTimeUnix >= min && x.AtDateTimeUnix <= max)
                .ToArrayAsync(cancellationToken: stoppingToken);
            bool IsTimeToCharge = PvpcCronBackgroundService.IsTimeToCharge(todayPvpcViews, timeToCheckDateTimeOffset, allowWhenKWhPriceInEurosBelow: 0.07M);

            TuyaDevice[] Devices = await dbCxt.TuyaDevices.ToArrayAsync(cancellationToken: stoppingToken);

            for (int i = 0; i < Devices.Length; i++)
            {
                TuyaDevice tuyaDevice = Devices[i];
                var tuyaDeviceBase = tuyaDevice.ToTuyaDeviceBase();
                _ = IsTimeToCharge ? tuyaDeviceBase.TurnOn() : tuyaDeviceBase.TurnOff();
            }
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        logger.LogDebug("End {ApplicationName}", AppName);
    }
}
