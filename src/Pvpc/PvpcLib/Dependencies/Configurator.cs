using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Seedysoft.PvpcLib.Services;
using Seedysoft.PvpcLib.Settings;

namespace Seedysoft.PvpcLib.Dependencies;

internal sealed class Configurator : UtilsLib.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.PvpcSettings.json", false, true)
            .AddJsonFile($"appsettings.TuyaManagerSettings.json", false, true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(PvpcSettings)).Get<PvpcSettings>()!);
        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(TuyaManagerSettings)).Get<TuyaManagerSettings>()!);
        hostApplicationBuilder.Services.TryAddSingleton<PvpcCronBackgroundService>();
        hostApplicationBuilder.Services.TryAddSingleton<TuyaManagerCronBackgroundService>();
    }
}
