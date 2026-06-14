using Microsoft.Extensions.Configuration;

namespace Seedysoft.Libs.Travel.Services;

public abstract class ServiceBase(IConfiguration configuration)
{
    protected Settings.TravelSettings TravelSettings => configuration
        .GetSection(nameof(Settings.TravelSettings)).Get<Settings.TravelSettings>()!;
}

// TODO                         Try to use better geocoding and routing services
