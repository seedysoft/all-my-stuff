using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.GasStationPricesSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.GasStationPricesSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddScoped<Services.GasStationPricesService>();

        _ = hostApplicationBuilder.Services.AddHttpClient();

        //_ = hostApplicationBuilder.Services.AddHttpClient("Default")
        //    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        //    {
        //        UseCookies = false
        //    });

        //_ = hostApplicationBuilder.Services.AddHttpClient("NoRedirectClient")
        //    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        //    {
        //        AllowAutoRedirect = false
        //    });

        //_ = hostApplicationBuilder.Services.AddHttpClient("CookiesClient")
        //    .ConfigurePrimaryHttpMessageHandler(() =>
        //    {
        //        return new HttpClientHandler
        //        {
        //            UseCookies = true,
        //            CookieContainer = new System.Net.CookieContainer()
        //        };
        //    });
    }
}
