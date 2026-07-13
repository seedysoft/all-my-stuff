using OneOf;

namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// Marker is used to display clickable/draggable icons on the map.
/// </summary>
public record class Marker : InteractiveLayer
{
    public Marker(LatLng position) => Position = position;

    /// <summary>
    /// Geographical point.
    /// </summary>
    [J("position")] public LatLng Position { get; }

    /// <summary>
    /// Text for the alt attribute of the icon image.
    /// <see href="https://leafletjs.com/examples/accessibility/#markers-must-be-labelled">Useful for accessibility.</see>
    /// </summary>
    /// <remarks>Default: 'Marker'</remarks>
    [J("alt")] public string? Alt { get; set; }

    /// <summary>
    /// When true, the map will pan whenever the marker is focused (via e.g. pressing tab on the keyboard) to ensure the marker is visible within the map's bounds.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("autoPanOnFocus")] public bool? AutoPanOnFocus { get; set; }

    /// <summary>
    /// Icon instance to use for rendering the marker.
    /// See <see href="https://leafletjs.com/reference-2.0.0.html#icon">Icon documentation</see> for details on how to customize the marker icon.
    /// If not specified, a common instance of <see cref="Icon.Default"/> is used.
    /// </summary>
    [J("icon")] public Icon? Icon { get; set; }

    /// <summary>
    /// Whether the marker can be tabbed to with a keyboard and clicked by pressing enter.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("keyboard")] public bool? Keyboard { get; set; }

    /// <summary>
    /// The opacity of the marker.
    /// </summary>
    /// <remarks>Default: 1.0</remarks>
    [J("opacity")] public double? Opacity { get; set; }

    /// <summary>
    /// Map pane where the markers icon will be added.
    /// </summary>
    /// <remarks>Default: 'markerPane'</remarks>
    [J("pane")] public new string? Pane { get; set; }

    /// <summary>
    /// The z-index offset used for the riseOnHover feature.
    /// </summary>
    /// <remarks>Default: 250</remarks>
    [J("riseOffset")] public double? RiseOffset { get; set; }

    /// <summary>
    /// If true, the marker will get on top of others when you hover the pointer over it.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("riseOnHover")] public bool? RiseOnHover { get; set; }

    /// <summary>
    /// Map pane where the markers shadow will be added.
    /// </summary>
    /// <remarks>Default: 'shadowPane'</remarks>
    [J("shadowPane")] public string? ShadowPane { get; set; }

    /// <summary>
    /// Text for the browser tooltip that appear on marker hover (no tooltip by default).
    /// <see href="https://leafletjs.com/examples/accessibility/#markers-must-be-labelled">Useful for accessibility.</see>
    /// </summary>
    /// <remarks>Default: ''</remarks>
    [J("title")] public string? Title { get; set; }

    /// <summary>
    /// By default, marker images zIndex is set automatically based on its latitude.
    /// Use this option if you want to put the marker on top of all others (or below), specifying a high value like 1000 (or high negative value, respectively).
    /// </summary>
    /// <remarks>Default: 0</remarks>
    [J("zIndexOffset")] public double? ZIndexOffset { get; set; }
}
