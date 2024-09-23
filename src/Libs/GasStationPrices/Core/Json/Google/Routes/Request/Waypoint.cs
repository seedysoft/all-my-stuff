namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Request;

/// <summary>
/// Encapsulates a waypoint. Waypoints mark both the beginning and end of a route, and include intermediate stops along the route.
/// </summary>
public class Waypoint
{
    /// <summary>
    /// Marks this waypoint as a milestone rather a stopping point.
    /// For each non-via waypoint in the request, the response appends an entry to the <see cref="Route.Legs"/>> array to provide the details for stopovers on that leg of the trip.
    /// Set this value to true when you want the route to pass through this waypoint without stopping over.
    /// Via waypoints don't cause an entry to be added to the legs array, but they do route the journey through the waypoint.
    /// You can only set this value on waypoints that are intermediates.
    /// The request fails if you set this field on terminal waypoints.
    /// If <see cref="Body.OptimizeWaypointOrder"/> is set to true then this field cannot be set to true; otherwise, the request fails.
    /// </summary>
    [J("via"), I(Condition = C.WhenWritingNull)] public bool? Via { get; set; }
    /// <summary>
    /// Indicates that the waypoint is meant for vehicles to stop at, where the intention is to either pickup or drop-off.
    /// When you set this value, the calculated route won't include non-via waypoints on roads that are unsuitable for pickup and drop-off.
    /// This option works only for DRIVE and TWO_WHEELER travel modes, and when the locationType is <see cref="Location"/>.
    /// </summary>
    [J("vehicleStopover"), I(Condition = C.WhenWritingNull)] public bool? VehicleStopover { get; set; }
    /// <summary>
    /// Indicates that the location of this waypoint is meant to have a preference for the vehicle to stop at a particular side of road.
    /// When you set this value, the route will pass through the location so that the vehicle can stop at the side of road that the location is biased towards from the center of the road.
    /// This option works only for <see cref="RouteTravelMode.DRIVE"/> and <see cref="RouteTravelMode.TWO_WHEELER"/>.
    /// </summary>
    [J("sideOfRoad"), I(Condition = C.WhenWritingNull)] public bool? SideOfRoad { get; set; }
    /// <summary>
    /// A point specified using geographic coordinates, including an optional heading.
    /// </summary>
    [J("location"), I(Condition = C.WhenWritingNull)] public Shared.Location? Location { get; set; }
    /// <summary>
    /// The POI Place ID associated with the waypoint.
    /// </summary>
    [J("placeId"), I(Condition = C.WhenWritingNull)] public string? PlaceId { get; set; }
    /// <summary>
    /// Human readable address or a plus code. See <see href="https://plus.codes"/> for details.
    /// </summary>
    [J("address"), I(Condition = C.WhenWritingNull)] public string? Address { get; set; }
}
