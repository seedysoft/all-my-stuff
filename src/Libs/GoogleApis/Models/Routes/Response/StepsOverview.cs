﻿namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Provides overview information about a list of <see cref="RouteLegStep"/>s.
/// </summary>
public record StepsOverview
{
    /// <summary>
    /// Summarized information about different multi-modal segments of the <see cref="RouteLeg.Steps"/>.
    /// This field is not populated if the <see cref="RouteLeg"/> does not contain any multi-modal segments in the steps.
    /// </summary>
    [J("multiModalSegments"), I(Condition = C.WhenWritingNull)]
    public MultiModalSegment[]? MultiModalSegments { get; init; }
}
