using System.Text.Json.Serialization;

namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// Preferences for <see cref="RouteTravelMode.TRANSIT"/> based routes that influence the route that is returned.
/// </summary>
public class TransitPreferences
{
    /// <summary>
    /// A set of travel modes to use when getting a <see cref="RouteTravelMode.TRANSIT"/> route. Defaults to all supported modes of travel.
    /// </summary>
    [J("allowedTravelModes")] public TransitTravelMode[]? AllowedTravelModes { get; set; }
    /// <summary>
    /// A routing preference that, when specified, influences the <see cref="RouteTravelMode.TRANSIT"/> route returned.
    /// </summary>
    [J("routingPreference")][K(typeof(JsonStringEnumConverter))] public TransitRoutingPreference? RoutingPreference { get; set; }
}
