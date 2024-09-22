namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// Encapsulates a location (a geographic point, and an optional heading).
/// </summary>
public class Location
{
    /// <summary>
    /// The waypoint's geographic coordinates.
    /// </summary>
    [J("latLng")] public required LatitudeLongitude LatLng { get; set; }
    /// <summary>
    /// The compass heading associated with the direction of the flow of traffic.
    /// This value specifies the side of the road for pickup and drop-off.
    /// Heading values can be from 0 to 360, where 0 specifies a heading of due North, 90 specifies a heading of due East, and so on.
    /// You can use this field only for DRIVE and TWO_WHEELER RouteTravelMode.
    /// </summary>
    [J("heading")][I(Condition = C.WhenWritingNull)] public long? Heading { get; set; }
}
