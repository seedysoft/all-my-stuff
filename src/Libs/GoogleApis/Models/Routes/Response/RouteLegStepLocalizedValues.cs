﻿namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Text representations of certain properties.
/// </summary>
public record RouteLegStepLocalizedValues
{
    /// <summary>
    /// Travel distance represented in text form.
    /// </summary>
    [J("distance"), I(Condition = C.WhenWritingNull)]
    public LocalizedText? Distance { get; init; }

    /// <summary>
    /// Duration without taking traffic conditions into consideration, represented in text form.
    /// </summary>
    [J("staticDuration"), I(Condition = C.WhenWritingNull)]
    public LocalizedText? StaticDuration { get; init; }
}
