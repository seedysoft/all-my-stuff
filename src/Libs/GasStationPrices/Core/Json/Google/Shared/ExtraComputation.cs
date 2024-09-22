namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// Extra computations to perform while completing the request.
/// </summary>
public enum ExtraComputation
{
    /// <summary>
    /// Not used. Requests containing this value will fail.
    /// </summary>
    EXTRA_COMPUTATION_UNSPECIFIED,
    /// <summary>
    /// Toll information for the route(s).
    /// </summary>
    TOLLS,
    /// <summary>
    /// Estimated fuel consumption for the route(s).
    /// </summary>
    FUEL_CONSUMPTION,
    /// <summary>
    /// Traffic aware polylines for the route(s).
    /// </summary>
    TRAFFIC_ON_POLYLINE,
    /// <summary>
    /// <see href="https://developers.google.com/maps/documentation/routes/reference/rest/v2/TopLevel/google.maps.routing.v2.NavigationInstructions.instructions">NavigationInstructions</see> presented as a formatted HTML text string. This content is meant to be read as-is. This content is for display only. Do not programmatically parse it.
    /// </summary>
    HTML_FORMATTED_NAVIGATION_INSTRUCTIONS,
}
