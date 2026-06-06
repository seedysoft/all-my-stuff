using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Geography.Services;

public class RoutesService(IConfiguration configuration, ILogger<RoutesService> logger) : GeographyServiceBase(configuration)
{
    public async Task<List<Models.RouteModel>> GetRoutesAsync(ViewModels.TravelQueryModel model, CancellationToken cancellationToken)
    {
        Routers.RouterBase router = GeographySettings.RouteSettings.RouteImplementation switch
        {
            "CartoCiudad" => new Routers.CartoCiudadRouter(GeographySettings.RouteSettings.RoutesApi.First(x => x.RouteName == "CartoCiudad"), logger),
            "OSRM" => new Routers.OsrmRouter(GeographySettings.RouteSettings.RoutesApi.First(x => x.RouteName == "OSRM"), logger),
            _ => throw new InvalidOperationException($"Unsupported router: {GeographySettings.RouteSettings.RouteImplementation}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using router implementation: {RouterImplementation}", GeographySettings.RouteSettings.RouteImplementation);

        return await router.GetRoutesAsync(model, cancellationToken);
    }
}
