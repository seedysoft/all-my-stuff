namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Contains the additional information that the user should be informed about, such as possible traffic zone restrictions on a leg step.
/// </summary>
public class RouteLegStepTravelAdvisory
{
    /// <summary>
    /// NOTE: This field is not currently populated.
    /// </summary>
    [J("speedReadingIntervals"), I(Condition = C.WhenWritingNull)]
    public SpeedReadingInterval[]? SpeedReadingIntervals { get; set; }
}
