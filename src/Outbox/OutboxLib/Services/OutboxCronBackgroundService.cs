using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.OutboxLib.Services;

public class OutboxCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
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
    private static void ConfigJsonFile(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment) { }
    private static void ConfigServices(IServiceCollection services, IConfiguration configuration) => _ = services.AddHostedService<OutboxCronBackgroundService>();

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<OutboxCronBackgroundService> Logger;

    public OutboxCronBackgroundService(
        TelegramLib.Settings.TelegramSettings config
        , IServiceProvider serviceProvider
        , ILogger<OutboxCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Called {ApplicationName} version {Version}", ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        await TelegramLib.Main.SendMessagesAsync(ServiceProvider, Logger, CancellationToken.None);

        Logger.LogInformation("End {ApplicationName}", ServiceProvider.GetRequiredService<IHostEnvironment>().ApplicationName);
    }
}
