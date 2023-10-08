using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.ObtainPvpcLib.Services;

public class ObtainPvpCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private static bool isConfigured;

    public static void Configure(IHostBuilder hostBuilder)
    {
        if (!isConfigured)
        {
            _ = hostBuilder
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) => ConfigJsonFile(configurationBuilder, hostBuilderContext.HostingEnvironment))

                .ConfigureServices((hostBuilderContext, services) => ConfigServices(services, hostBuilderContext.Configuration));

            isConfigured = true;
        }
    }
    public static void Configure(IConfigurationBuilder configurationBuilder, IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        if (!isConfigured)
        {
            ConfigJsonFile(configurationBuilder, hostEnvironment);

            ConfigServices(services, configuration);

            isConfigured = true;
        }
    }
    private static void ConfigJsonFile(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment) =>
        _ = configurationBuilder.AddJsonFile($"appsettings.ObtainPvpcSettings.json", false, true);
    private static void ConfigServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(configuration.GetSection(nameof(Settings.ObtainPvpcSettings)).Get<Settings.ObtainPvpcSettings>()!);

        _ = services.AddHostedService<ObtainPvpCronBackgroundService>();
    }

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<ObtainPvpCronBackgroundService> Logger;

    public ObtainPvpCronBackgroundService(
        Settings.ObtainPvpcSettings config
        , IServiceProvider serviceProvider
        , ILogger<ObtainPvpCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Called {ApplicationName} version {Version}", ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

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

        Logger.LogInformation("End {ApplicationName}", ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName);
    }
}
