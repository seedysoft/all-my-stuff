using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Travel.Services.Routing;

public class RoutingService(IConfiguration configuration, ILogger<RoutingService> logger) : ServiceBase(configuration)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="dest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(
        Models.Location orig
        , Models.Location dest
        , CancellationToken cancellationToken)
    {
        Settings.RoutingApi api = TravelSettings.RoutingSettings.RoutingApis.First(x => x.Name == TravelSettings.RoutingSettings.CurrentImplementation);

        RoutingBase RoutingImplementation = TravelSettings.RoutingSettings.CurrentImplementation switch
        {
#pragma warning disable format
            //Settings.RoutingImplementations.Google                     => new GoogleRoutes(api, logger),
        
            //Settings.RoutingImplementations.MapboxDirections           => new MapboxDirectionsRouter(api, logger),
        
            Settings.RoutingImplementations.OpenSourceRoutingMachine    => new OpenSourceRoutingMachine(api, logger),

            Settings.RoutingImplementations.Valhalla                    => new ValhallaRoutingService(new ValhallaRoutingApi(api) , logger),
#pragma warning restore format

            _ => throw new InvalidOperationException($"Unsupported router: {TravelSettings.RoutingSettings.CurrentImplementation}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using routing implementation: {RoutingImplementation}", TravelSettings.RoutingSettings.CurrentImplementation);

        return await RoutingImplementation.GetRoutesAsync(orig, dest, cancellationToken);
    }
}
