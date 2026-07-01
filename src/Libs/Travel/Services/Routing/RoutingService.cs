using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Travel.Services.Routing;

/// <summary>
/// Provides routing services that abstract different routing implementations (OSRM, Valhalla, etc.)
/// and deliver optimized routes between geographic locations.
/// </summary>
/// <remarks>
/// This service uses a factory pattern to instantiate the appropriate routing implementation
/// based on the configured <see cref="Settings.TravelSettings.RoutingSettings.CurrentImplementation"/>.
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
    /// Thrown when the configured routing implementation in <see cref="Settings.TravelSettings.RoutingSettings.CurrentImplementation"/>
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
        Settings.RoutingApi api = TravelSettings.RoutingSettings.RoutingApis.First(x => x.Name == TravelSettings.RoutingSettings.CurrentImplementation);

        Implementations.RoutingImplementationBase RoutingImplementation = TravelSettings.RoutingSettings.CurrentImplementation switch
        {
#pragma warning disable format
            //Settings.RoutingImplementations.Google                     => new GoogleRoutes(api, logger),
        
            //Settings.RoutingImplementations.MapboxDirections           => new MapboxDirectionsRouter(api, logger),
        
            Settings.RoutingImplementations.OpenSourceRoutingMachine    => new Implementations.OsrmRoutingService(api, logger),

            Settings.RoutingImplementations.Valhalla                    => new Implementations.ValhallaRoutingService(new Implementations.ValhallaRoutingApi(api) , logger),
#pragma warning restore format

            _ => throw new InvalidOperationException($"Unsupported router: {TravelSettings.RoutingSettings.CurrentImplementation}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using routing implementation: {RoutingImplementation}", TravelSettings.RoutingSettings.CurrentImplementation);

        return await RoutingImplementation.GetRoutesAsync(orig, dest, cancellationToken);
    }
}
