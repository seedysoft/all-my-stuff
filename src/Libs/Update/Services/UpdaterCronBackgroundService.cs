using Microsoft.Extensions.DependencyInjection;

namespace Seedysoft.Libs.Update.Services;

public sealed class UpdaterCronBackgroundService : BackgroundServices.Cron
{
    private static readonly TimeSpan FiveSecondsTimeSpan = TimeSpan.FromSeconds(5);
    private readonly Microsoft.Extensions.Logging.ILogger<UpdaterCronBackgroundService> Logger;

    public UpdaterCronBackgroundService(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        Logger = ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<UpdaterCronBackgroundService>>();

        Config = new Libs.BackgroundServices.ScheduleConfig() { CronExpression = "7 * * * *" /*At every 7th minute*/ };
    }

    public override Task DoWorkAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
