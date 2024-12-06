using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.GoogleApis.Dependencies;

internal sealed class Configurator : Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.GoogleApisSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.GoogleApisSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton(
            hostApplicationBuilder.Configuration.GetSection(nameof(Settings.GoogleApisSettings)).Get<Settings.GoogleApisSettings>()!);

        hostApplicationBuilder.Services.TryAddScoped<Services.PlacesService>();
    }
}
