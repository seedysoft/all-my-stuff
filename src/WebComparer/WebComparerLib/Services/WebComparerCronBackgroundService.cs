using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.WebComparerLib.Services;

public class WebComparerCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private readonly string ApplicationName ;

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<WebComparerCronBackgroundService> Logger;

    public WebComparerCronBackgroundService(
        Settings.WebComparerSettings config
        , IServiceProvider serviceProvider
        , ILogger<WebComparerCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;

        ApplicationName = serviceProvider.GetRequiredService<IHostEnvironment>().ApplicationName;
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Called {ApplicationName} version {Version}", ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        DbContexts.DbCxt dbCtx = ServiceProvider.GetRequiredService<DbContexts.DbCxt>();

        await Main.FindDifferencesAsync(dbCtx, Logger);

        Logger.LogInformation("End {ApplicationName}", ApplicationName);
    }
}
