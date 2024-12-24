namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Provides summarized information about different multi-modal segments of the <see cref="RouteLeg.Steps"/>.
/// A multi-modal segment is defined as one or more contiguous <see cref="RouteLegStep"/> that have the same <see cref="Shared.RouteTravelMode"/>.
/// This field is not populated if the <see cref="RouteLeg"/> does not contain any multi-modal segments in the steps.
/// </summary>
public record MultiModalSegment
{
    /// <summary>
    /// NavigationInstruction for the multi-modal segment.
    /// </summary>
    [J("navigationInstruction"), I(Condition = C.WhenWritingNull)]
    public NavigationInstruction? NavigationInstruction { get; init; }

    /// <summary>
    /// The travel mode of the multi-modal segment.
    /// </summary>
    [J("travelMode"), I(Condition = C.WhenWritingNull)]
    public Shared.RouteTravelMode? TravelMode { get; init; }

    /// <summary>
    /// The corresponding RouteLegStep index that is the start of a multi-modal segment.
    /// </summary>
    [J("stepStartIndex"), I(Condition = C.WhenWritingNull)]
    public int? StepStartIndex { get; init; }

    /// <summary>
    /// The corresponding RouteLegStep index that is the end of a multi-modal segment.
    /// </summary>
    [J("stepEndIndex"), I(Condition = C.WhenWritingNull)]
    public int? StepEndIndex { get; init; }
}
