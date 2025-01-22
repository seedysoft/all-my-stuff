namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Contains a segment between non-via waypoints.
/// </summary>
public class RouteLeg
{
    /// <summary>
    /// The travel distance of the route leg, in meters.
    /// </summary>
    [J("distanceMeters"), I(Condition = C.WhenWritingNull)]
    public int? DistanceMeters { get; init; }

    /// <summary>
    /// The length of time needed to navigate the leg.
    /// If the <see cref="Request.RoutingPreference"> is set to <see cref="Request.RoutingPreference.TrafficUnaware"/>, then this value is the same as <see cref="StaticDuration">.
    /// If the <see cref="Request.RoutingPreference"> is either <see cref="Request.RoutingPreference.TrafficAware/> or <see cref="Request.RoutingPreference.TrafficAwareOptimal"/>, then this value is calculated taking traffic conditions into account.
    /// A duration in seconds with up to nine fractional digits, ending with 's'. Example: "3.5s".
    /// </summary>
    [J("duration"), I(Condition = C.WhenWritingNull)]
    public string? Duration { get; init; }

    /// <summary>
    /// The duration of travel through the leg, calculated without taking traffic conditions into consideration.
    /// A duration in seconds with up to nine fractional digits, ending with 's'. Example: "3.5s".
    /// </summary>
    [J("staticDuration"), I(Condition = C.WhenWritingNull)]
    public string? StaticDuration { get; init; }

    /// <summary>
    /// The overall polyline for this leg that includes each step's polyline.
    /// </summary>
    [J("polyline")]
    public required Polyline Polyline { get; init; }

    /// <summary>
    /// The start location of this leg. 
    /// This location might be different from the provided origin.
    /// For example, when the provided origin is not near a road, this is a point on the road.
    /// </summary>
    [J("startLocation"), I(Condition = C.WhenWritingNull)]
    public Models.Shared.Location? StartLocation { get; init; }

    /// <summary>
    /// The end location of this leg.
    /// This location might be different from the provided destination.
    /// For example, when the provided destination is not near a road, this is a point on the road.
    /// </summary>
    [J("endLocation"), I(Condition = C.WhenWritingNull)]
    public Models.Shared.Location? EndLocation { get; init; }

    /// <summary>
    /// An array of steps denoting segments within this leg.
    /// Each step represents one navigation instruction.
    /// </summary>
    [J("steps"), I(Condition = C.WhenWritingNull)]
    public RouteLegStep[]? Steps { get; init; }

    /// <summary>
    /// Contains the additional information that the user should be informed about, such as possible traffic zone restrictions, on a route leg.
    /// </summary>
    [J("travelAdvisory"), I(Condition = C.WhenWritingNull)]
    public RouteLegTravelAdvisory? TravelAdvisory { get; init; }

    /// <summary>
    /// Text representations of properties of the RouteLeg.
    /// </summary>
    [J("localizedValues"), I(Condition = C.WhenWritingNull)]
    public RouteLegLocalizedValues? LocalizedValues { get; init; }

    /// <summary>
    /// Overview information about the steps in this RouteLeg.
    /// This field is only populated for TRANSIT routes.
    /// </summary>
    [J("stepsOverview"), I(Condition = C.WhenWritingNull)]
    public StepsOverview? StepsOverview { get; init; }
}
