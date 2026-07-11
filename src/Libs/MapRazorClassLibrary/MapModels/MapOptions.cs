namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

// TODO                            USE MAP OPTIONS
public sealed class MapOptions
{
    // 			// @option layers: Layer[] = []
    // 			// Array of layers that will be added to the map initially
    // 			layers: [],
    // 
    // 			// @option maxBounds: LatLngBounds = null
    // 			// When this option is set, the map restricts the view to the given
    // 			// geographical bounds, bouncing the user back if the user tries to pan
    // 			// outside the view. To set the restriction dynamically, use
    // 			// [`setMaxBounds`](#map-setmaxbounds) method.
    // 			maxBounds: undefined,
    // 
    // 			// @option renderer: Renderer = *
    // 			// The default method for drawing vector layers on the map. `SVG`
    // 			// or `Canvas` by default depending on browser support.
    // 			renderer: undefined,
    // 
    // 			// @section Animation Options
    // 			// @option zoomAnimation: Boolean = true
    // 			// Whether the map zoom animation is enabled. By default it's enabled
    // 			// in all browsers that support CSS Transitions except Android.
    // 			zoomAnimation: true,
    // 
    // 			// @option zoomAnimationThreshold: Number = 4
    // 			// Won't animate zoom if the zoom difference exceeds this value.
    // 			zoomAnimationThreshold: 4,
    // 
    // 			// @option fadeAnimation: Boolean = true
    // 			// Whether the tile fade animation is enabled. By default it's enabled
    // 			// in all browsers that support CSS Transitions except Android.
    // 			fadeAnimation: true,
    // 
    // 			// @option markerZoomAnimation: Boolean = true
    // 			// Whether markers animate their zoom with the zoom animation, if disabled
    // 			// they will disappear for the length of the animation. By default it's
    // 			// enabled in all browsers that support CSS Transitions except Android.
    // 			markerZoomAnimation: true,
    // 
    // 			// @option transform3DLimit: Number = 2^23
    // 			// Defines the maximum size of a CSS translation transform. The default
    // 			// value should not be changed unless a web browser positions layers in
    // 			// the wrong place after doing a large `panBy`.
    // 			transform3DLimit: 8388608, // Precision limit of a 32-bit float
    // 
    // 			// @section Interaction Options
    // 			// @option zoomSnap: Number = 1
    // 			// Forces the map's zoom level to always be a multiple of this, particularly
    // 			// right after a [`fitBounds()`](#map-fitbounds) or a pinch-zoom.
    // 			// By default, the zoom level snaps to the nearest integer; lower values
    // 			// (e.g. `0.5` or `0.1`) allow for greater granularity. A value of `0`
    // 			// means the zoom level will not be snapped after `fitBounds` or a pinch-zoom.
    // 			zoomSnap: 1,
    // 
    // 			// @option zoomDelta: Number = 1
    // 			// Controls how much the map's zoom level will change after a
    // 			// [`zoomIn()`](#map-zoomin), [`zoomOut()`](#map-zoomout), pressing `+`
    // 			// or `-` on the keyboard, or using the [zoom controls](#control-zoom).
    // 			// Values smaller than `1` (e.g. `0.5`) allow for greater granularity.
    // 			zoomDelta: 1,
    // 
    // 			// @option trackResize: Boolean = true
    // 			// Whether the map automatically handles browser window resize to update itself.
    // 			trackResize: true

    ///// <summary>
    ///// The <see href="https://leafletjs.com/reference-2.0.0.html#crs">Coordinate Reference System</see> to use.
    ///// Don't change this if you're not sure what it means.
    ///// </summary>
    ///// <remarks>Default: CRS.EPSG3857</remarks>
    //[J("crs")] public Crs? Crs { get; set; } = Crs.EPSG3857;

    /// <summary>
    /// Initial geographic center of the map
    /// </summary>
    /// <remarks>Default: undefined</remarks>
    [J("center")] public LatLng? Center { get; set; }

    /// <summary>
    /// Initial map zoom level
    /// </summary>
    /// <remarks>Default: undefined</remarks>
    [J("zoom")] public int? Zoom { get; set; }

    ///// <summary>
    ///// Minimum zoom level of the map.
    ///// If not specified and at least one <see cref="GridLayer"/> or <see cref="TileLayer"/> is in the map, the lowest of their minZoom options will be used instead.
    ///// </summary>
    ///// <remarks>Default: undefined</remarks>
    //[J("minZoom")] public int? MinZoom { get; set; }

    ///// <summary>
    ///// Maximum zoom level of the map.
    ///// If not specified and at least one <see cref="GridLayer"/> or <see cref="TileLayer"/> is in the map, the highest of their maxZoom options will be used instead.
    ///// </summary>
    ///// <remarks>Default: undefined</remarks>
    //[J("maxZoom")] public int? MaxZoom { get; set; }

    ///// <summary>
    ///// Whether a attribution control is added to the map by default.
    ///// </summary>
    ///// <remarks>Default: true</remarks>
    //[J("attributionControl")] public bool AttributionControl { get; set; } = true;

    ///// <summary>
    ///// Whether a zoom control is added to the map by default.
    ///// </summary>
    ///// <remarks>Default: true</remarks>
    //[J("zoomControl")] public bool ZoomControl { get; set; } = true;

    ///// <summary>
    ///// Whether the map can be zoomed by using the mouse wheel.
    ///// If passed 'center', it will zoom to the center of the view regardless of where the pointer was.
    ///// </summary>
    ///// <remarks>Default: true</remarks>
    //[J("scrollWheelZoom")] public OneOf<bool, string> ScrollWheelZoom { get; set; } = true;

    ///// <summary>
    ///// Whether the map can be zoomed in by double clicking on it and zoomed out by double clicking while holding shift.
    ///// If passed 'center', double-click zoom will zoom to the center of the view regardless of where the pointer was.
    ///// </summary>
    ///// <remarks>Default: true</remarks>
    //[J("doubleClickZoom")] public OneOf<bool, string> DoubleClickZoom { get; set; } = true;

    ///// <summary>
    ///// 	Whether the map is draggable with pointer or not.
    ///// </summary>
    ///// <remarks>Default: true</remarks>
    //[J("dragging")] public bool Dragging { get; set; } = true;

    ///// <summary>
    ///// hether Paths should be rendered on a Canvas renderer.
    ///// By default, all Paths are rendered in a SVG renderer.
    ///// </summary>
    ///// <remarks>Default: false</remarks>
    //[J("preferCanvas")] public bool PreferCanvas { get; set; } = false;

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
