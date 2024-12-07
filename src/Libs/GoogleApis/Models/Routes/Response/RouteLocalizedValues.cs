namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Text representations of certain properties.
/// </summary>
public class RouteLocalizedValues : RouteLegStepLocalizedValues
{
    /// <summary>
    /// Duration taking traffic conditions into consideration, represented in text form. Note: If you did not request traffic information, this value will be the same value as staticDuration.
    /// </summary>
    [J("duration"), I(Condition = C.WhenWritingNull)]
    public LocalizedText? Duration { get; set; }

    /// <summary>
    /// Transit fare represented in text form.
    /// </summary>
    [J("transitFare"), I(Condition = C.WhenWritingNull)]
    public LocalizedText? TransitFare { get; set; }
}
