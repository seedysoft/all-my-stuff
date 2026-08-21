using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.Travel.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.TravelSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.TravelSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
        ;
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddScoped<Services.Geocoding.GeocodingService>();
        hostApplicationBuilder.Services.TryAddScoped<Services.Routing.RoutingService>();

        _ = hostApplicationBuilder.Services.AddHttpClient(name: Microsoft.Extensions.Options.Options.DefaultName)
            .ConfigureHttpClient(static configureClient =>
            {
                configureClient.DefaultRequestHeaders.Accept.Clear();
                configureClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                configureClient.DefaultRequestHeaders.AcceptEncoding.Clear();
                configureClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                configureClient.DefaultRequestHeaders.Connection.Clear();
                configureClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
                configureClient.DefaultRequestHeaders.UserAgent.Clear();
                configureClient.DefaultRequestHeaders.UserAgent.ParseAdd("MiAppEnNet10/1.0 (Windows 10; Contacto: tu-email@dominio.com)");
            })
            .ConfigurePrimaryHttpMessageHandler(static () =>
            {
                HttpClientHandler handler = new()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,
                };

                return handler;
            })
            ;
    }
}
