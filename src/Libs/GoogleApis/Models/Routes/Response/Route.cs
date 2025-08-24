namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Contains a route, which consists of a series of connected road segments that join beginning, ending, and intermediate waypoints.
/// </summary>
public class Route
{
    /// <summary>
    /// Labels for the Route that are useful to identify specific properties of the route to compare against others.
    /// </summary>
    [J("routeLabels"), I(Condition = C.WhenWritingNull), K(typeof(Core.Extensions.EnumMemberArrayJsonConverter<RouteLabel>))]
    public RouteLabel[]? RouteLabels { get; init; }

    /// <summary>
    /// A collection of legs (path segments between waypoints) that make up the route.
    /// Each leg corresponds to the trip between two non-via <see cref="Request.Waypoint"/>s.
    /// For example, a route with no intermediate waypoints has only one leg.
    /// A route that includes one non-via intermediate waypoint has two legs.
    /// A route that includes one via intermediate waypoint has one leg.
    /// The order of the legs matches the order of waypoints from origin to intermediates to destination.
    /// </summary>
    [J("legs")]
    public required RouteLeg[] Legs { get; init; }

    /// <summary>
    /// The travel distance of the route, in meters.
    /// </summary>
    [J("distanceMeters"), I(Condition = C.WhenWritingNull)]
    public int? DistanceMeters { get; init; }

    [J("duration"), I(Condition = C.WhenWritingNull)]
    public string? Duration { get; init; }

    [J("staticDuration"), I(Condition = C.WhenWritingNull)]
    public string? StaticDuration { get; init; }

    [J("polyline"), I(Condition = C.WhenWritingNull)]
    public Polyline? Polyline { get; init; }

    [J("description"), I(Condition = C.WhenWritingNull)]
    public string? Description { get; init; }

    [J("warnings"), I(Condition = C.WhenWritingNull)]
    public string[]? Warnings { get; init; }

    [J("viewport"), I(Condition = C.WhenWritingNull)]
    public Viewport? Viewport { get; init; }

    [J("travelAdvisory"), I(Condition = C.WhenWritingNull)]
    public RouteTravelAdvisory? TravelAdvisory { get; init; }

    [J("localizedValues"), I(Condition = C.WhenWritingNull)]
    public RouteLocalizedValues? LocalizedValues { get; init; }

    [J("routeToken"), I(Condition = C.WhenWritingNull)]
    public string? RouteToken { get; init; }
}
