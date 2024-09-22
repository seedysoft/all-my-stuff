namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Labels for the <see cref="Route"/> that are useful to identify specific properties of the route to compare against others.
/// </summary>
public enum RouteLabel
{
    /// <summary>
    /// Default - not used.
    /// </summary>
    ROUTE_LABEL_UNSPECIFIED,
    /// <summary>
    /// The default "best" route returned for the route computation.
    /// </summary>
    DEFAULT_ROUTE,
    /// <summary>
    /// An alternative to the default "best" route. Routes like this will be returned when <see cref="Request.Body.ComputeAlternativeRoutes"/> is specified.
    /// </summary>
    DEFAULT_ROUTE_ALTERNATE,
    /// <summary>
    /// Fuel efficient route. Routes labeled with this value are determined to be optimized for Eco parameters such as fuel consumption.
    /// </summary>
    FUEL_EFFICIENT,
}
