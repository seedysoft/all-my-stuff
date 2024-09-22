namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

public enum RoutingPreference
{
    /// <summary>
    /// Computes routes without taking live traffic conditions into consideration. Suitable when traffic conditions don't matter or are not applicable. Using this value produces the lowest latency. Note: For <see cref="RouteTravelMode.DRIVE"/> and <see cref="RouteTravelMode.TWO_WHEELER"/>, the route and duration chosen are based on road network and average time-independent traffic conditions, not current road conditions. Consequently, routes may include roads that are temporarily closed. Results for a given request may vary over time due to changes in the road network, updated average traffic conditions, and the distributed nature of the service. Results may also vary between nearly-equivalent routes at any time or frequency.
    /// </summary>
    TRAFFIC_UNAWARE,
    /// <summary>
    /// Calculates routes taking live traffic conditions into consideration. In contrast to <see cref="TRAFFIC_AWARE_OPTIMAL"/>, some optimizations are applied to significantly reduce latency.
    /// </summary>
    TRAFFIC_AWARE,
    /// <summary>
    /// Calculates the routes taking live traffic conditions into consideration, without applying most performance optimizations. Using this value produces the highest latency.
    /// </summary>
    TRAFFIC_AWARE_OPTIMAL,
    /// <summary>
    /// No routing preference specified. Default to <see cref="TRAFFIC_UNAWARE"/>.
    /// </summary>
    ROUTING_PREFERENCE_UNSPECIFIED = TRAFFIC_UNAWARE,
}
