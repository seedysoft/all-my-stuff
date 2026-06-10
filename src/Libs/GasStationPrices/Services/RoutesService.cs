using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.GasStationPrices.Services.Routers;
using Seedysoft.Libs.GasStationPrices.Settings;
using Seedysoft.Libs.GasStationPrices.ViewModels;

namespace Seedysoft.Libs.GasStationPrices.Services;

public class RoutesService(IConfiguration configuration, ILogger<RoutesService> logger) : GeographyServiceBase(configuration)
{
    /// <summary>
    /// Obtiene rutas entre dos puntos utilizando la API de OpenRouteService.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IList<(string NombreRuta, double[][] Coordenadas)>> GetRoutesAsync(TravelQueryModel model, CancellationToken cancellationToken)
    {
        RouteApi? api = GeographySettings.RouteSettings.RouteApis.FirstOrDefault(x => x.RouteName == GeographySettings.RouteSettings.CurrentImplementation);

        RouterBase router = GeographySettings.RouteSettings.CurrentImplementation switch
        {
            #pragma warning disable format
            
            //Settings.RouteImplementations.CartoCiudad       => new Routers.CartoCiudadRouter(api!, logger),

            //Settings.RouteImplementations.Google            => new Routers.GoogleRoutes(api!, logger),
            
            //Settings.RouteImplementations.MapboxDirections  => new Routers.MapboxDirectionsRouter(api!, logger),
            
            RouteImplementations.OSRM              => new OsrmRouter(api!, logger),

            _ => throw new InvalidOperationException($"Unsupported router: {GeographySettings.RouteSettings.CurrentImplementation}"),
            
            #pragma warning restore format
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using router implementation: {RouterImplementation}", GeographySettings.RouteSettings.CurrentImplementation);

        return await router.GetRoutesAsync(model, cancellationToken);
    }
}
