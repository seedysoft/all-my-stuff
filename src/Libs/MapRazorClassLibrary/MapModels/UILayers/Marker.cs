namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.UILayers;

/// <summary>
/// Marker is used to display clickable/draggable icons on the map.
/// </summary>
public sealed record class Marker : Base.InteractiveLayer
{
    public Marker(Basic.LatLng position) : base()
    {
        Position = position;
        BubblingPointerEvents = false;
        Pane = Map.Panes.MarkerPane;
    }

    /// <summary>
    /// Geographical point.
    /// </summary>
    [J("position")] public Basic.LatLng Position { get; }

    /// <summary>
    /// Icon instance to use for rendering the marker.
    /// See <see href="https://leafletjs.com/reference-2.0.0.html#icon">Icon documentation</see> for details on how to customize the marker icon.
    /// If not specified, a common instance of <see cref="Basic.Icon.Default"/> is used.
    /// </summary>
    /// <remarks>Default: *</remarks>
    [J("icon")] public Basic.Icon? Icon { get; set; }

    /// <summary>
    /// Whether the marker can be tabbed to with a keyboard and clicked by pressing enter.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("keyboard")] public bool? Keyboard { get; set; } = true;

    /// <summary>
    /// Text for the browser tooltip that appear on marker hover (no tooltip by default).
    /// <see href="https://leafletjs.com/examples/accessibility/#markers-must-be-labelled">Useful for accessibility.</see>
    /// </summary>
    /// <remarks>Default: ''</remarks>
    [J("title")] public string? Title { get; set; } = string.Empty;

    /// <summary>
    /// Text for the alt attribute of the icon image.
    /// <see href="https://leafletjs.com/examples/accessibility/#markers-must-be-labelled">Useful for accessibility.</see>
    /// </summary>
    /// <remarks>Default: 'Marker'</remarks>
    [J("alt")] public string? Alt { get; set; } = "Marker";

    /// <summary>
    /// By default, marker images zIndex is set automatically based on its latitude.
    /// Use this option if you want to put the marker on top of all others (or below), specifying a high value like 1000 (or high negative value, respectively).
    /// </summary>
    /// <remarks>Default: 0</remarks>
    [J("zIndexOffset")] public double? ZIndexOffset { get; set; } = 0;

    /// <summary>
    /// The opacity of the marker.
    /// </summary>
    /// <remarks>Default: 1.0</remarks>
    [J("opacity")] public double? Opacity { get; set; } = 1.0;

    /// <summary>
    /// If true, the marker will get on top of others when you hover the pointer over it.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("riseOnHover")] public bool? RiseOnHover { get; set; } = false;

    /// <summary>
    /// The z-index offset used for the riseOnHover feature.
    /// </summary>
    /// <remarks>Default: 250</remarks>
    [J("riseOffset")] public double? RiseOffset { get; set; } = 250;

    /// <summary>
    /// Map pane where the markers shadow will be added.
    /// </summary>
    /// <remarks>Default: 'shadowPane'</remarks>
    [J("shadowPane")] public string? ShadowPane { get; set; } = "shadowPane";

    /// <summary>
    /// When <c>true</c>, the map will pan whenever the marker is focused (via e.g. pressing tab on the keyboard) to ensure the marker is visible within the map's bounds.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("autoPanOnFocus")] public bool? AutoPanOnFocus { get; set; } = true;

    // Draggable marker options
    // Option	Type	Default	Description
    // draggable	Boolean	false	Whether the marker is draggable with pointer or not.
    // autoPan	Boolean	false	Whether to pan the map when dragging this marker near its edge or not.
    // autoPanPadding	Point	Point(50, 50)	Distance (in pixels to the left/right and to the top/bottom) of the map edge to start panning the map.
    // autoPanSpeed	Number	10	Number of pixels the map should pan by.

}
