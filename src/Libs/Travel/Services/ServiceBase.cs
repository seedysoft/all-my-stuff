using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Seedysoft.Libs.Travel.Services;

public abstract class ServiceBase(IServiceProvider serviceProvider, IConfiguration configuration)
{
    protected Settings.TravelSettings TravelSettings => configuration
        .GetSection(nameof(Settings.TravelSettings)).Get<Settings.TravelSettings>()!;

    protected IHttpClientFactory HttpClientFactory =>
        serviceProvider.GetRequiredService<IHttpClientFactory>();
}
