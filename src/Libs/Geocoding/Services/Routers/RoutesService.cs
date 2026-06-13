using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Geocoding.Services.Routers;

public class RoutesService(IConfiguration configuration, ILogger<RoutesService> logger) : GeographyServiceBase(configuration)
{
    /// <summary>
    /// Obtiene rutas entre dos puntos utilizando la API de OpenRouteService.
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="dest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(Models.Location orig, Models.Location dest, CancellationToken cancellationToken)
    {
        Settings.RouteApi api = GeographySettings.RouteSettings.RouteApis.First(x => x.RouteName == GeographySettings.RouteSettings.CurrentImplementation);

        RouterBase router = GeographySettings.RouteSettings.CurrentImplementation switch
        {
#pragma warning disable format
            //Settings.RouteImplementations.CartoCiudad                => new Routers.CartoCiudadRouter(api, logger),

            //Settings.RouteImplementations.Google                     => new Routers.GoogleRoutes(api, logger),
            
            //Settings.RouteImplementations.MapboxDirections           => new Routers.MapboxDirectionsRouter(api, logger),
            
            Settings.RouteImplementations.OpenSourceRoutingMachine   => new OsrmRouter(api, logger),
#pragma warning restore format

            _ => throw new InvalidOperationException($"Unsupported router: {GeographySettings.RouteSettings.CurrentImplementation}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using router implementation: {RouterImplementation}", GeographySettings.RouteSettings.CurrentImplementation);

        return await router.GetRoutesAsync(orig, dest, cancellationToken);
    }
}
