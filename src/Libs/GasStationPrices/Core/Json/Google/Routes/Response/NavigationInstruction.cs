﻿using System.Text.Json.Serialization;

namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Encapsulates navigation instructions for a <see cref="RouteLegStep"/>.
/// </summary>
public class NavigationInstruction
{
    /// <summary>
    /// Encapsulates the navigation instructions for the current step (for example, turn left, merge, or straight). This field determines which icon to display.
    /// </summary>
    [J("maneuver")][I(Condition = C.WhenWritingNull)][K(typeof(JsonStringEnumConverter))] public Maneuver? Maneuver { get; set; }
    /// <summary>
    /// Instructions for navigating this step.
    /// </summary>
    [J("instructions")][I(Condition = C.WhenWritingNull)] public string? Instructions { get; set; }
}