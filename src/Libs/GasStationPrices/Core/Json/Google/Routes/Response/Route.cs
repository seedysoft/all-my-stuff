namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Contains a route, which consists of a series of connected road segments that join beginning, ending, and intermediate waypoints.
/// </summary>
public class Route
{
    /// <summary>
    /// Labels for the Route that are useful to identify specific properties of the route to compare against others.
    /// </summary>
    [J("routeLabels"), I(Condition = C.WhenWritingNull)] public RouteLabel[]? RouteLabels { get; set; }
    /// <summary>
    /// A collection of legs (path segments between waypoints) that make up the route.
    /// Each leg corresponds to the trip between two non-via <see cref="Request.Waypoint"/>s.
    /// For example, a route with no intermediate waypoints has only one leg.
    /// A route that includes one non-via intermediate waypoint has two legs.
    /// A route that includes one via intermediate waypoint has one leg.
    /// The order of the legs matches the order of waypoints from origin to intermediates to destination.
    /// </summary>
    [J("legs")] public required RouteLeg[] Legs { get; set; }
    /// <summary>
    /// The travel distance of the route, in meters.
    /// </summary>
    [J("distanceMeters"), I(Condition = C.WhenWritingNull)] public int? DistanceMeters { get; set; }
    [J("duration"), I(Condition = C.WhenWritingNull)] public string? Duration { get; set; }
    [J("staticDuration"), I(Condition = C.WhenWritingNull)] public string? StaticDuration { get; set; }
    [J("polyline"), I(Condition = C.WhenWritingNull)] public Polyline? Polyline { get; set; }
    [J("description"), I(Condition = C.WhenWritingNull)] public string? Description { get; set; }
    [J("warnings"), I(Condition = C.WhenWritingNull)] public string[]? Warnings { get; set; }
    [J("viewport"), I(Condition = C.WhenWritingNull)] public Viewport? Viewport { get; set; }
    [J("travelAdvisory"), I(Condition = C.WhenWritingNull)] public RouteTravelAdvisory? TravelAdvisory { get; set; }
    [J("localizedValues"), I(Condition = C.WhenWritingNull)] public RouteLocalizedValues? LocalizedValues { get; set; }
    [J("routeToken"), I(Condition = C.WhenWritingNull)] public string? RouteToken { get; set; }
}
