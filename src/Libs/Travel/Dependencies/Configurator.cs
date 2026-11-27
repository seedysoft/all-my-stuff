using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.Travel.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        //string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        //_ = hostApplicationBuilder.Configuration
        //    .AddJsonFile($"appsettings.{nameof(Settings.TravelSettings)}.json", optional: false, reloadOnChange: true)
        //    .AddJsonFile($"appsettings.{nameof(Settings.TravelSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
        //;
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddTransient<Interfaces.IGeocodingService, Services.NominatimService>();
        hostApplicationBuilder.Services.TryAddTransient<Interfaces.IReverseService, Services.NominatimService>();
        hostApplicationBuilder.Services.TryAddTransient<Interfaces.IRoutingService, Services.OsrmService>();

        _ = Services.NominatimService.ConfigureHttpClient(hostApplicationBuilder);

        _ = Services.OsrmService.ConfigureHttpClient(hostApplicationBuilder);
    }
}
