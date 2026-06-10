using Microsoft.Extensions.Configuration;
using Seedysoft.Libs.GasStationPrices.Settings;

namespace Seedysoft.Libs.GasStationPrices.Services;

public abstract class GeographyServiceBase(IConfiguration configuration)
{
    protected GeographySettings GeographySettings => configuration
        .GetSection(nameof(Settings.GeographySettings)).Get<GeographySettings>()!;
}
