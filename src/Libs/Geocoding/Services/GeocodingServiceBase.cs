using Microsoft.Extensions.Configuration;

namespace Seedysoft.Libs.Geocoding.Services;

public abstract class GeocodingServiceBase(IConfiguration configuration)
{
    protected Settings.GeocodingSettings GeocodingSettings => configuration
        .GetSection(nameof(Settings.GeocodingSettings)).Get<Settings.GeocodingSettings>()!;
}

// TODO                         Try to use better geocoding and route services

// TODO                         Change BaseMapLayer to OpenStreetMap
