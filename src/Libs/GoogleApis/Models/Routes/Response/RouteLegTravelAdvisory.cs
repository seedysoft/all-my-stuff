namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Contains the additional information that the user should be informed about on a leg step, such as possible traffic zone restrictions.
/// </summary>
public record RouteLegTravelAdvisory
{
    /// <summary>
    /// Contains information about tolls on the specific RouteLeg.
    /// This field is only populated if we expect there are tolls on the RouteLeg.
    /// If this field is set but the estimatedPrice subfield is not populated, we expect that road contains tolls but we do not know an estimated price.
    /// If this field does not exist, then there is no toll on the RouteLeg.
    /// </summary>
    [J("tollInfo"), I(Condition = C.WhenWritingNull)]
    public TollInfo? TollInfo { get; init; }

    /// <summary>
    /// Speed reading intervals detailing traffic density.
    /// Applicable in case of TRAFFIC_AWARE and TRAFFIC_AWARE_OPTIMAL routing preferences.
    /// The intervals cover the entire polyline of the RouteLeg without overlap.
    /// The start point of a specified interval is the same as the end point of the preceding interval.
    /// </summary>
    [J("speedReadingIntervals"), I(Condition = C.WhenWritingNull)]
    public SpeedReadingInterval[]? SpeedReadingIntervals { get; init; }
}
