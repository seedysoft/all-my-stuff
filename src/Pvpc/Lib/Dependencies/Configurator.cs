using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Seedysoft.Pvpc.Lib.Dependencies;

internal sealed class Configurator : Libs.Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.PvpcSettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.TuyaManagerSettings.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(Settings.PvpcSettings)).Get<Settings.PvpcSettings>()!);
        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(Settings.TuyaManagerSettings)).Get<Settings.TuyaManagerSettings>()!);
        hostApplicationBuilder.Services.TryAddSingleton<Services.PvpcCronBackgroundService>();
        hostApplicationBuilder.Services.TryAddSingleton<Services.TuyaManagerCronBackgroundService>();
    }
}
