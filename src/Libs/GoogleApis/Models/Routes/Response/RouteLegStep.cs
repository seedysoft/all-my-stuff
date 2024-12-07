namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Contains a segment of a <see cref="RouteLeg"/>. A step corresponds to a single navigation instruction. Route legs are made up of steps.
/// </summary>
public class RouteLegStep
{
    /// <summary>
    /// The travel distance of the route leg, in meters.
    /// </summary>
    [J("distanceMeters"), I(Condition = C.WhenWritingNull)]
    public int? DistanceMeters { get; set; }

    /// <summary>
    /// The duration of travel through this step without taking traffic conditions into consideration. In some circumstances, this field might not have a value. A duration in seconds with up to nine fractional digits, ending with 's'. Example: "3.5s".
    /// </summary>
    [J("staticDuration"), I(Condition = C.WhenWritingNull)]
    public string? StaticDuration { get; set; }

    /// <summary>
    /// The polyline associated with this step.
    /// </summary>
    [J("polyline"), I(Condition = C.WhenWritingNull)]
    public Polyline? Polyline { get; set; }

    /// <summary>
    /// The start location of this step.
    /// </summary>
    [J("startLocation"), I(Condition = C.WhenWritingNull)]
    public Models.Shared.Location? StartLocation { get; set; }

    /// <summary>
    /// The end location of this step.
    /// </summary>
    [J("endLocation"), I(Condition = C.WhenWritingNull)]
    public Models.Shared.Location? EndLocation { get; set; }

    /// <summary>
    /// The end location of this step.
    /// </summary>
    [J("navigationInstruction"), I(Condition = C.WhenWritingNull)]
    public NavigationInstruction? NavigationInstruction { get; set; }

    /// <summary>
    /// Contains the additional information that the user should be informed about, such as possible traffic zone restrictions, on a leg step.
    /// </summary>
    [J("travelAdvisory"), I(Condition = C.WhenWritingNull)]
    public RouteLegStepTravelAdvisory? TravelAdvisory { get; set; }

    /// <summary>
    /// Text representations of properties of the RouteLegStep.
    /// </summary>
    [J("localizedValues"), I(Condition = C.WhenWritingNull)]
    public RouteLegStepLocalizedValues? LocalizedValues { get; set; }

    /// <summary>
    /// Details pertaining to this step if the travel mode is TRANSIT.
    /// </summary>
    [J("transitDetails"), I(Condition = C.WhenWritingNull)]
    public RouteLegStepTransitDetails? TransitDetails { get; set; }

    /// <summary>
    /// The travel mode used for this step.
    /// </summary>
    [J("travelMode"), I(Condition = C.WhenWritingNull)]
    public Shared.RouteTravelMode? TravelMode { get; set; }
}
