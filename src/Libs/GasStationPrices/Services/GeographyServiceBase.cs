using Microsoft.Extensions.Configuration;

namespace Seedysoft.Libs.GasStationPrices.Services;

public abstract class GeographyServiceBase(IConfiguration configuration)
{
    protected Settings.GeographySettings GeographySettings => configuration
        .GetSection(nameof(Settings.GeographySettings)).Get<Settings.GeographySettings>()!;
}
