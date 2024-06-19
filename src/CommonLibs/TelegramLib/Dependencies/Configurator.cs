using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Seedysoft.TelegramLib.Dependencies;

internal sealed class Configurator : UtilsLib.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.TelegramSettings.json", false, true)
            .AddJsonFile($"appsettings.TelegramSettings.{hostApplicationBuilder.Environment.EnvironmentName}.json", false, true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(Settings.TelegramSettings)).Get<Settings.TelegramSettings>()!);
        hostApplicationBuilder.Services.TryAddSingleton<Services.TelegramHostedService>();
    }
}
