namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// The central class of the API — it is used to create a map on a page and manipulate it.
/// </summary>
public record Map : Evented
{
    /// <summary>
    /// Whether <see cref="Path"/>s should be rendered on a <see cref="Canvas"/> renderer.
    /// By default, all <see cref="Path"/>s are rendered in a <see cref="SVG"/> renderer.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("preferCanvas")] public bool? PreferCanvas { get; set; } = false;

    #region Control options

    ///// <summary>
    /// Whether a attribution control is added to the map by default.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("attributionControl")] public bool? AttributionControl { get; set; } = true;

    /// <summary>
    /// Whether a zoom control is added to the map by default.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("zoomControl")] public bool? ZoomControl { get; set; } = true;

    #endregion

    #region Interaction Options

    /// <summary>
    /// Whether the map can be zoomed in by double clicking on it and zoomed out by double clicking while holding shift.
    /// If passed 'center', double-click zoom will zoom to the center of the view regardless of where the pointer was.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("doubleClickZoom"), K(typeof(OneOf.Serialization.SystemTextJson.OneOfJsonConverter))]
    public OneOf.OneOf<bool, string>? DoubleClickZoom { get; set; } = true;

    #endregion

    #region Panning Inertia Options

    #endregion

    #region Keyboard Navigation Options

    #endregion

    #region Touch interaction options

    #endregion

    #region Mouse wheel options

    ///// <summary>
    ///// Whether the map can be zoomed by using the mouse wheel.
    ///// If passed 'center', it will zoom to the center of the view regardless of where the pointer was.
    ///// </summary>
    ///// <remarks>Default: <c>true</c></remarks>
    //[J("scrollWheelZoom")] public OneOf<bool, string> ScrollWheelZoom { get; set; } = true;

    #endregion

    #region Map State Options

    /// <summary>
    /// Initial geographic center of the map.
    /// </summary>
    /// <remarks>Default: undefined</remarks>
    [J("center")] public LatLng? Center { get; set; }

    /// <summary>
    /// Initial map zoom level
    /// </summary>
    /// <remarks>Default: undefined</remarks>
    [J("zoom")] public double? Zoom { get; set; }

    /// <summary>
    /// Minimum zoom level of the map.
    /// If not specified and at least one <see cref="GridLayer"/> or <see cref="TileLayer"/> is in the map, the lowest of their minZoom options will be used instead.
    /// </summary>
    /// <remarks>Default: *</remarks>
    [J("minZoom")] public double? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom level of the map.
    /// If not specified and at least one <see cref="GridLayer"/> or <see cref="TileLayer"/> is in the map, the highest of their maxZoom options will be used instead.
    /// </summary>
    /// <remarks>Default: *</remarks>
    [J("maxZoom")] public double? MaxZoom { get; set; }

    #endregion

    #region Animation Options

    #endregion

    ///// <summary>
    ///// Whether the map is draggable with pointer or not.
    ///// </summary>
    ///// <remarks>Default: <c>true</c></remarks>
    //[J("dragging")] public bool Dragging { get; set; } = true;

    //public List<TileLayer> TileLayers { get; set; } = [];

    //public LayersControlOptions? LayersControl { get; set; }

    //public ScaleControlOptions? ScaleControl { get; set; }
}

//public abstract class ControlOptions
//{
//    public enum ControlPosition
//    {
//        [J("topleft")] TopLeft,
//        [J("topright")] TopRight,
//        [J("bottomleft")] BottomLeft,
//        [J("bottomright")] BottomRight
//    }

//    public ControlPosition? Position { get; set; }
//}

//public sealed class LayersControlOptions : ControlOptions
//{

//    /// <summary>
//    /// If true, the control will be collapsed into an icon and expanded on mouse hover, touch, or keyboard activation.
//    /// </summary>
//    /// <remarks>Default is <see langword="true"/></remarks>
//    public bool? Collapsed { get; set; } = true;

//    /// <summary>
//    /// If true, the control will assign zIndexes in increasing order to all of its layers so that the order is preserved when switching them on/off.
//    /// </summary>
//    /// <remarks>Default is <see langword="true"/></remarks>
//    public bool? AutoZIndex { get; set; } = true;

//    /// <summary>
//    /// If true, the base layers in the control will be hidden when there is only one.
//    /// </summary>
//    /// <remarks>Default is <see langword="false"/></remarks>
//    public bool? HideSingleBase { get; set; }

//    /// <summary>
//    /// Whether to sort the layers. When false, layers will keep the order in which they were added to the control.
//    /// </summary>
//    /// <remarks>Default is <see langword="false"/></remarks>
//    public bool? SortLayers { get; set; }
//}

//public class ScaleControlOptions : ControlOptions
//{
//    /// <summary>
//    /// Maximum width of the control in pixels. The width is set dynamically to show round values (e.g. 100, 200, 500).
//    /// </summary>
//    /// <remarks>Default is 100.</remarks>
//    public int? MaxWidth { get; set; } = 100;

//    /// <summary>
//    /// Whether to show the imperial scale line (mi/ft).
//    /// </summary>
//    /// <remarks>Default is <see langword="true"/></remarks>
//    public bool? Imperial { get; set; } = true;

//    /// <summary>
//    /// Whether to show the metric scale line (m/km).
//    /// </summary>
//    /// <remarks>Default is <see langword="true"/></remarks>
//    public bool? Metric { get; set; }

//    /// <summary>
//    /// If true, the control is updated on moveend, otherwise it's always up-to-date (updated on move).
//    /// </summary>
//    /// <remarks>Default is <see langword="false"/></remarks>
//    public bool? UpdateWhenIdle { get; set; }
//}
