using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Utils.Extensions;
using Seedysoft.Pvpc.Lib.Extensions;
using System.Diagnostics;

namespace Seedysoft.Pvpc.Lib.Services;

public sealed class TuyaManagerBackgroundServiceCron : Libs.BackgroundServices.Cron
{
    private readonly ILogger<TuyaManagerBackgroundServiceCron> Logger;

    public TuyaManagerBackgroundServiceCron(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime) : base(serviceProvider, hostApplicationLifetime)
    {
        Logger = ServiceProvider.GetRequiredService<ILogger<TuyaManagerBackgroundServiceCron>>();

        Config = ServiceProvider.GetRequiredService<Settings.TuyaManagerSettings>();
    }

    private Settings.TuyaManagerSettings Settings => (Settings.TuyaManagerSettings)Config;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // Execute on init
        _ = Task.Factory.StartNew(async () => await DoWorkAsync(cancellationToken), cancellationToken);

        _ = base.StartAsync(cancellationToken);

        await Task.CompletedTask;
    }

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        string? AppName = GetType().FullName;

        logger.LogDebug("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            Libs.Infrastructure.DbContexts.DbCxt dbCxt = ServiceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCxt>();

            Libs.Core.Entities.TuyaDevice[] Devices = await dbCxt.TuyaDevices.AsNoTracking().ToArrayAsync(cancellationToken: stoppingToken);

            DateTimeOffset timeToCheckDateTimeOffset = DateTimeOffset.Now;
            DateTimeOffset dateToQueryDateTimeOffset = timeToCheckDateTimeOffset.Subtract(timeToCheckDateTimeOffset.TimeOfDay);

            Libs.Core.Entities.Pvpc[] PricesForDayPvpcs = await dbCxt.Pvpcs.AsNoTracking()
                .Where(x => x.AtDateTimeOffset >= dateToQueryDateTimeOffset)
                .Where(x => x.AtDateTimeOffset < dateToQueryDateTimeOffset.AddDays(1))
                .ToArrayAsync(cancellationToken: stoppingToken);

            bool IsTimeToCharge = PvpcBackgroundServiceCron.IsTimeToCharge(PricesForDayPvpcs, timeToCheckDateTimeOffset, Settings);

            for (int i = 0; i < Devices.Length; i++)
            {
                Libs.Core.Entities.TuyaDevice tuyaDevice = Devices[i];
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
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        logger.LogDebug("End {ApplicationName}", AppName);
    }
}
