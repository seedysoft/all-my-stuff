namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Actual routing mode used for returned fallback response.
/// </summary>
public enum FallbackRoutingMode
{
    /// <summary>
    /// Not used.
    /// </summary>
    FALLBACK_ROUTING_MODE_UNSPECIFIED,
    /// <summary>
    /// Indicates the <see cref="Body.RoutingPreference.TRAFFIC_UNAWARE"/> <see cref="Request.Body.RoutingPreference"/> was used to compute the response.
    /// </summary>
    FALLBACK_TRAFFIC_UNAWARE,
    /// <summary>
    /// Indicates the <see cref="Body.RoutingPreference.TRAFFIC_AWARE"/> <see cref="Request.Body.RoutingPreference"/> was used to compute the response.
    /// </summary>
    FALLBACK_TRAFFIC_AWARE,
}
