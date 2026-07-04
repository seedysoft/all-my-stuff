using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Travel.Services.Routing;

/// <summary>
/// Provides routing services that abstract different routing implementations (OSRM, Valhalla, etc.)
/// and deliver optimized routes between geographic locations.
/// </summary>
/// <remarks>
/// This service uses a factory pattern to instantiate the appropriate routing implementation
/// based on the configured <see cref="TravelSettings.RoutingSettings.CurrentImpl"/>.
/// Supported implementations include:
/// <list type="bullet">
/// <item><description>Open Source Routing Machine (OSRM)</description></item>
/// <item><description>Valhalla Routing Engine</description></item>
/// </list>
/// </remarks>
public class RoutingService(IConfiguration configuration, ILogger<RoutingService> logger) : ServiceBase(configuration)
{
    /// <summary>
    /// Retrieves a collection of optimized routes between the specified origin and destination.
    /// </summary>
    /// <param name="orig">The starting location for the route.</param>
    /// <param name="dest">The destination location for the route.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A read-only list of tuples containing:
    /// <list type="bullet">
    /// <item><description><c>NombreRuta</c> - The name or identifier of the route</description></item>
    /// <item><description><c>Coordenadas</c> - A 2D array of geographic coordinates representing the route path</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the configured routing implementation in <see cref="TravelSettings.RoutingSettings.CurrentImpl"/>
    /// is not supported by this service.
    /// </exception>
    /// <remarks>
    /// This method logs informational messages indicating which routing implementation is being used
    /// when logging is enabled at the <see cref="LogLevel.Information"/> level.
    /// </remarks>
    public async Task<IReadOnlyList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(
        Models.Location orig
        , Models.Location dest
        , CancellationToken cancellationToken)
    {
        Settings.RoutingServiceApi api = TravelSettings.RoutingSettings.RoutingApis.First(x => x.Name == TravelSettings.RoutingSettings.CurrentImplName);

        Impl.RoutingServiceImplBase RoutingImpl = TravelSettings.RoutingSettings.CurrentImplName switch
        {
#pragma warning disable format
            //Settings.RoutingImplName.Google                     => new GoogleRoutes(api, logger),
        
            //Settings.RoutingImplName.MapboxDirections           => new MapboxDirectionsRouter(api, logger),
        
            Settings.RoutingImplName.OpenSourceRoutingMachine    => new Impl.OsrmRoutingServiceImpl(api, logger),

            Settings.RoutingImplName.Valhalla                    => new Impl.ValhallaRoutingServiceImpl (new Impl.ValhallaRoutingApi(api) , logger),
#pragma warning restore format

            _ => throw new InvalidOperationException($"Unsupported RoutingServiceImpl: {TravelSettings.RoutingSettings.CurrentImplName}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using routing implementation: {RoutingImpl}", TravelSettings.RoutingSettings.CurrentImplName);

        return await RoutingImpl.GetRoutesAsync(orig, dest, cancellationToken);
    }
}
