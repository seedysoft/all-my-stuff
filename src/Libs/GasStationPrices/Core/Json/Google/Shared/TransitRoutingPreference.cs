namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// Specifies routing preferences for transit routes.
/// </summary>
public enum TransitRoutingPreference
{
    /// <summary>
    /// No preference specified.
    /// </summary>
    TRANSIT_ROUTING_PREFERENCE_UNSPECIFIED,
    /// <summary>
    /// Indicates that the calculated route should prefer limited amounts of walking.
    /// </summary>
    LESS_WALKING,
    /// <summary>
    /// Indicates that the calculated route should prefer a limited number of transfers.
    /// </summary>
    FEWER_TRANSFERS,
}
