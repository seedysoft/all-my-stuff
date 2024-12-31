using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Seedysoft.Libs.Update.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.UpdateSettings)}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
        => hostApplicationBuilder.Services.TryAddSingleton<Services.UpdaterCronBackgroundService>();
}
