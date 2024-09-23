namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Contains a segment between non-via waypoints.
/// </summary>
public class RouteLeg
{
    /// <summary>
    /// The travel distance of the route leg, in meters.
    /// </summary>
    [J("distanceMeters"), I(Condition = C.WhenWritingNull)] public int? DistanceMeters { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("duration"), I(Condition = C.WhenWritingNull)] public string? Duration { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("staticDuration"), I(Condition = C.WhenWritingNull)] public string? StaticDuration { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("polyline")] public required Polyline Polyline { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("startLocation"), I(Condition = C.WhenWritingNull)] public Shared.Location? StartLocation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("endLocation"), I(Condition = C.WhenWritingNull)] public Shared.Location? EndLocation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("steps"), I(Condition = C.WhenWritingNull)] public RouteLegStep[]? Steps { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("travelAdvisory"), I(Condition = C.WhenWritingNull)] public RouteLegTravelAdvisory? TravelAdvisory { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("localizedValues"), I(Condition = C.WhenWritingNull)] public RouteLegLocalizedValues? LocalizedValues { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [J("stepsOverview"), I(Condition = C.WhenWritingNull)] public StepsOverview? StepsOverview { get; set; }
}
