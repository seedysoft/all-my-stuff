using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.ObtainPvpcLib.Services;

public class ObtainPvpCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private readonly string ApplicationName ;

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<ObtainPvpCronBackgroundService> Logger;

    public ObtainPvpCronBackgroundService(
        Settings.ObtainPvpcSettings config
        , IServiceProvider serviceProvider
        , ILogger<ObtainPvpCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;

        ApplicationName = serviceProvider.GetRequiredService<IHostEnvironment>().ApplicationName;
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Called {ApplicationName} version {Version}", ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        DateTime ForDate = DateTimeOffset.UtcNow.AddDays(1).Date;
        Logger.LogInformation("Requesting PVPCs for {ForDate}", ForDate.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat));

        DbContexts.DbCxt dbCtx = ServiceProvider.GetRequiredService<DbContexts.DbCxt>();

        int? HowManyPricesObtained = await Main.ObtainPricesAsync(
        dbCtx,
        (Settings.ObtainPvpcSettings)Config,
        Logger,
        ForDate,
        cancellationToken);
        Logger.LogInformation("Obtained {HowManyPricesObtained} PVPCs", HowManyPricesObtained);

        Logger.LogInformation("End {ApplicationName}", ApplicationName);
    }
}