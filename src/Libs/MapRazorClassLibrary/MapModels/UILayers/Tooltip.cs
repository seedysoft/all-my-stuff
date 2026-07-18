namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.UILayers;

/// <summary>
/// Used to display small texts on top of map layers.
/// </summary>
public sealed record class Tooltip : DivOverlay
{
    public Tooltip() : base() => Pane = Map.Panes.TooltipPane;

    /// <summary>
    /// Direction where to open the tooltip.
    /// Possible values are: right, left, top, bottom, center, auto.
    /// auto will dynamically switch between right and left according to the tooltip position on the map.
    /// </summary>
    /// <remarks>Default: 'auto'</remarks>
    [J("direction")] public string? Direction { get; set; } = "auto";

    /// <summary>
    /// Whether to open the tooltip permanently or only on pointerover.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("permanent")] public bool? Permanent { get; set; } = false;

    /// <summary>
    /// If <c>true</c>, the tooltip will follow the pointer instead of being fixed at the feature center.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("sticky")] public bool? Sticky { get; set; } = false;

    /// <summary>
    /// Tooltip container opacity.
    /// </summary>
    /// <remarks>Default: 0.9</remarks>
    [J("opacity")] public double? Opacity { get; set; } = 0.9;
}
