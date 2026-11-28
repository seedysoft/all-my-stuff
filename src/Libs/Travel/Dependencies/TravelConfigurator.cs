using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.Travel.Dependencies;

public sealed class TravelConfigurator : Core.Dependencies.ConfiguratorBase
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
        IHttpClientBuilder httpClientBuilder;
        httpClientBuilder = hostApplicationBuilder.Services.AddHttpClient<Interfaces.IGeocodingService, Services.NominatimService>();
        ConfigHttpClient(httpClientBuilder);
        httpClientBuilder = hostApplicationBuilder.Services.AddHttpClient<Interfaces.IReverseService, Services.NominatimService>();
        ConfigHttpClient(httpClientBuilder);
        httpClientBuilder = hostApplicationBuilder.Services.AddHttpClient<Interfaces.IRoutingService, Services.OsrmService>();
        ConfigHttpClient(httpClientBuilder);

        hostApplicationBuilder.Services.TryAddSingleton<Interfaces.IGeocodingService, Services.NominatimService>();
        hostApplicationBuilder.Services.TryAddSingleton<Interfaces.IReverseService, Services.NominatimService>();
        hostApplicationBuilder.Services.TryAddSingleton<Interfaces.IRoutingService, Services.OsrmService>();

        static void ConfigHttpClient(IHttpClientBuilder httpClientBuilder)
        {
            _ = httpClientBuilder

                .ConfigureHttpClient(static client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/json, text/plain, */*");
                    client.DefaultRequestHeaders.AcceptEncoding.Clear();
                    client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                    client.DefaultRequestHeaders.Connection.Clear();
                    client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
                    //configureClient.DefaultRequestHeaders.Host = System.Net.Dns.GetHostName(); // Do not use
                    client.DefaultRequestHeaders.UserAgent.Clear();
                    client.DefaultRequestHeaders.UserAgent.ParseAdd($"{nameof(Travel)}/1.0 (Windows 10; Contact: seedysoft@gmail.com)");
                })

                .ConfigurePrimaryHttpMessageHandler(static () =>
                {
                    return new HttpClientHandler()
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.All,
                        AllowAutoRedirect = true,
                        ClientCertificateOptions = ClientCertificateOption.Automatic,
                        PreAuthenticate = true,
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        SslProtocols = System.Security.Authentication.SslProtocols.None,
                    };
                });
        }
    }
}
