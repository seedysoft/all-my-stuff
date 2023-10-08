using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.WebComparerLib.Services;

public class WebComparerCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private static bool isConfigured;

    public static void Configure(IHostBuilder hostBuilder)
    {
        if (!isConfigured)
        {
            _ = hostBuilder
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) => ConfigJsonFile(configurationBuilder, hostBuilderContext.HostingEnvironment))

                .ConfigureServices((hostBuilderContext, services) => ConfigServices(services, hostBuilderContext.Configuration));

            TelegramLib.Services.TelegramService.Configure(hostBuilder);

            isConfigured = true;
        }
    }
    public static void Configure(IConfigurationBuilder configurationBuilder, IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        if (!isConfigured)
        {
            ConfigJsonFile(configurationBuilder, hostEnvironment);

            ConfigServices(services, configuration);

            TelegramLib.Services.TelegramService.Configure(configurationBuilder, services, configuration, hostEnvironment);

            isConfigured = true;
        }
    }
    private static void ConfigJsonFile(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment) =>
        _ = configurationBuilder.AddJsonFile($"appsettings.WebComparerSettings.{hostEnvironment.EnvironmentName}.json", false, true);
    private static void ConfigServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(configuration.GetSection(nameof(Settings.WebComparerSettings)).Get<Settings.WebComparerSettings>()!);

        _ = services.AddHostedService<WebComparerCronBackgroundService>();
    }

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<WebComparerCronBackgroundService> Logger;

    public WebComparerCronBackgroundService(
        Settings.WebComparerSettings config
        , IServiceProvider serviceProvider
        , ILogger<WebComparerCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Called {ApplicationName} version {Version}", ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        DbContexts.DbCxt dbCtx = ServiceProvider.GetRequiredService<DbContexts.DbCxt>();

        await Main.FindDifferencesAsync(dbCtx, Logger);

        Logger.LogInformation("End {ApplicationName}", ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName);
    }
}
