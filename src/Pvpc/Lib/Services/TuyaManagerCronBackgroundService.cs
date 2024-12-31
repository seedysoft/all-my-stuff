﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Pvpc.Lib.Extensions;

namespace Seedysoft.Pvpc.Lib.Services;

public sealed class TuyaManagerCronBackgroundService : Libs.BackgroundServices.Cron
{
    private readonly ILogger<TuyaManagerCronBackgroundService> Logger;

    public TuyaManagerCronBackgroundService(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        Logger = ServiceProvider.GetRequiredService<ILogger<TuyaManagerCronBackgroundService>>();

        Config = ServiceProvider.GetRequiredService<Settings.TuyaManagerSettings>();
    }

    private Settings.TuyaManagerSettings Settings => (Settings.TuyaManagerSettings)Config;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // Execute on init in background (fire and forget), so no hang on start 
        _ = Task.Factory.StartNew(async () => await DoWorkAsync(cancellationToken), cancellationToken);

        _ = base.StartAsync(cancellationToken);

        await Task.CompletedTask;
    }

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        string? AppName = GetType().FullName;

        Logger.LogDebug("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            Libs.Infrastructure.DbContexts.DbCxt dbCxt = ServiceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCxt>();

            Libs.Core.Entities.TuyaDevice[] Devices = await dbCxt.TuyaDevices.AsNoTracking().ToArrayAsync(stoppingToken);

            DateTimeOffset timeToCheckDateTimeOffset = DateTimeOffset.Now;
            DateTimeOffset dateToQueryDateTimeOffset = timeToCheckDateTimeOffset.Subtract(timeToCheckDateTimeOffset.TimeOfDay);

            Libs.Core.Entities.Pvpc[] PricesForDayPvpcs = await dbCxt.Pvpcs.AsNoTracking()
                .Where(x => x.AtDateTimeOffset >= dateToQueryDateTimeOffset)
                .Where(x => x.AtDateTimeOffset < dateToQueryDateTimeOffset.AddDays(1))
                .ToArrayAsync(stoppingToken);

            bool IsTimeToCharge = PvpcCronBackgroundService.IsTimeToCharge(PricesForDayPvpcs, timeToCheckDateTimeOffset, Settings);

            for (int i = 0; i < Devices.Length; i++)
            {
                Libs.Core.Entities.TuyaDevice tuyaDevice = Devices[i];
                var tuyaDeviceBase = tuyaDevice.ToTuyaDeviceBase();
                //_ = tuyaDeviceBase.GetStatus();

                object? TurnResult = IsTimeToCharge ? tuyaDeviceBase.TurnOn() : tuyaDeviceBase.TurnOff();
                if (TurnResult is Dictionary<string, object> dict)
                {
                    System.Diagnostics.Debug.WriteLine($"Turn {IsTimeToCharge} results:");
                    foreach (KeyValuePair<string, object> kvp in dict)
                        System.Diagnostics.Debug.WriteLine($"\t'{kvp.Key}'='{kvp.Value}'");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Turn {IsTimeToCharge} result: '{TurnResult}'");
                }
            }
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogDebug("End {ApplicationName}", AppName);
    }
}
