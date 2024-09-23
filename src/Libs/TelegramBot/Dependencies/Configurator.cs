using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Seedysoft.Libs.TelegramBot.Dependencies;

internal sealed class Configurator : Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.TelegramSettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.TelegramSettings.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton(
            hostApplicationBuilder.Configuration.GetSection(nameof(Settings.TelegramSettings)).Get<Settings.TelegramSettings>()!);
        hostApplicationBuilder.Services.TryAddSingleton<Services.TelegramHostedService>();
    }
}
