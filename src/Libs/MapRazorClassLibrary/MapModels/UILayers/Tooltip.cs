namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.UILayers;

/// <summary>
/// Used to display small texts on top of map layers.
/// </summary>
internal sealed class Tooltip : DivOverlay
{
    public Tooltip() : base()
    {
        Pane = Map.Panes.TooltipPane;
        Offset = Basic.Point.Empty;
    }

    /// <summary>
    /// Direction where to open the tooltip.
    /// Possible values are: right, left, top, bottom, center, auto.
    /// auto will dynamically switch between right and left according to the tooltip position on the map.
    /// </summary>
    /// <remarks>Default: <c>'auto'</c></remarks>
    [J("direction")] public Directions? Direction { get; set; } = Directions.Auto;

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
    /// <remarks>Default: <c>0.9</c></remarks>
    [J("opacity")] public double? Opacity { get; set; } = 0.9;

    [K(typeof(Core.Serialization.EnumMemberJsonConverter<Directions>))]
    public enum Directions
    {
#pragma warning disable format
        [System.Runtime.Serialization.EnumMember(Value = "right")]   Right,
        [System.Runtime.Serialization.EnumMember(Value = "left")]    Left,
        [System.Runtime.Serialization.EnumMember(Value = "top")]     Top,
        [System.Runtime.Serialization.EnumMember(Value = "bottom")]  Bottom,
        [System.Runtime.Serialization.EnumMember(Value = "center")]  Center,
        [System.Runtime.Serialization.EnumMember(Value = "auto")]    Auto,
#pragma warning restore format
    }
}
