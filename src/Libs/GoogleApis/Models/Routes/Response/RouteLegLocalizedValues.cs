namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Text representations of certain properties.
/// </summary>
public class RouteLegLocalizedValues
{
    /// <summary>
    /// Travel distance represented in text form.
    /// </summary>
    [J("distance"), I(Condition = C.WhenWritingNull)]
    public LocalizedText? Distance { get; set; }

    /// <summary>
    /// Duration taking traffic conditions into consideration represented in text form. Note: If you did not request traffic information, this value will be the same value as staticDuration.
    /// </summary>
    [J("duration"), I(Condition = C.WhenWritingNull)]
    public LocalizedText? Duration { get; set; }

    /// <summary>
    /// Duration without taking traffic conditions into consideration, represented in text form.
    /// </summary>
    [J("staticDuration"), I(Condition = C.WhenWritingNull)]
    public LocalizedText? StaticDuration { get; set; }
}
