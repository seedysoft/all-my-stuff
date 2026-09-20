using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;

namespace Seedysoft.Libs.GasStationPrices.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.GasStationPricesSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.GasStationPricesSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true)
        ;
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton<Services.GasStationPricesService>();

        _ = hostApplicationBuilder.Services.AddHttpClient(name: nameof(GasStationPrices))

            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                Services.GasStationPricesService gasStationPricesService = serviceProvider.GetRequiredService<Services.GasStationPricesService>();

                httpClient.BaseAddress = new Uri(gasStationPricesService.GasStationPricesSettings.Minetur.Urls.Base);

                //httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                //httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
                httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                //httpClient.DefaultRequestHeaders.Connection.Clear();
                httpClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
                //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{nameof(GasStationPrices)}/1.0 (Windows 10; Contact: seedysoft@gmail.com)");

                httpClient.Timeout = TimeSpan.FromMinutes(2);

            }) // ConfigureHttpClient

            .ConfigurePrimaryHttpMessageHandler(static () =>
            {
                HttpClientHandler handler = new()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                    AllowAutoRedirect = true,
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                    PreAuthenticate = true,
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                };

                return handler;
            }) // ConfigurePrimaryHttpMessageHandler
        ;
    }
}
