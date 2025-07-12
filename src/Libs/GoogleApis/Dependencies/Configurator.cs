using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.GoogleApis.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
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
#pragma warning disable IDE0022 // Use expression body for method
        //hostApplicationBuilder.Services.TryAddScoped<Services.DirectionsService>();
        hostApplicationBuilder.Services.TryAddScoped<Services.PlacesService>();
        //hostApplicationBuilder.Services.TryAddScoped<Services.RoutesService>();
#pragma warning restore IDE0022 // Use expression body for method
    }
}
