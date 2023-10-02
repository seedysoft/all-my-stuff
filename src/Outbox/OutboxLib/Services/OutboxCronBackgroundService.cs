using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.OutboxLib.Services;

public class OutboxCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private readonly string ApplicationName ;

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<OutboxCronBackgroundService> Logger;

    public OutboxCronBackgroundService(
        TelegramLib.Settings.TelegramSettings config
        , IServiceProvider serviceProvider
        , ILogger<OutboxCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;

        ApplicationName = serviceProvider.GetRequiredService<IHostEnvironment>().ApplicationName;
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Called {ApplicationName} version {Version}", ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        await TelegramLib.Main.SendMessagesAsync(ServiceProvider, Logger, CancellationToken.None);

        Logger.LogInformation("End {ApplicationName}", ApplicationName);
    }
}
