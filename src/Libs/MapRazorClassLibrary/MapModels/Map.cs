namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// The central class of the API — it is used to create a map on a page and manipulate it.
/// </summary>
public sealed record class Map : Base.Evented
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
    /// Set it to <c>false</c> if you don't want popups to close when user clicks the map.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("closePopupOnClick")] public bool? ClosePopupOnClick { get; set; } = true;

    /// <summary>
    /// Whether the map can be zoomed in by double clicking on it and zoomed out by double clicking while holding shift.
    /// If passed 'center', double-click zoom will zoom to the center of the view regardless of where the pointer was.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("doubleClickZoom"), K(typeof(OneOf.Serialization.SystemTextJson.OneOfJsonConverter))]
    public OneOf.OneOf<bool, string>? DoubleClickZoom { get; set; } = true;

    ///// <summary>
    /// Whether the map is draggable with pointer or not.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("dragging")] public bool? Dragging { get; set; } = true;

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
    /// <remarks>Default: <c>undefined</c></remarks>
    [J("center")] public Basic.LatLng? Center { get; set; }

    /// <summary>
    /// Initial map zoom level
    /// </summary>
    /// <remarks>Default: <c>undefined</c></remarks>
    [J("zoom")] public double? Zoom { get; set; }

    /// <summary>
    /// Minimum zoom level of the map.
    /// If not specified and at least one <see cref="GridLayer"/> or <see cref="TileLayer"/> is in the map, the lowest of their minZoom options will be used instead.
    /// </summary>
    /// <remarks>Default: <c>*</c></remarks>
    [J("minZoom")] public double? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom level of the map.
    /// If not specified and at least one <see cref="GridLayer"/> or <see cref="TileLayer"/> is in the map, the highest of their maxZoom options will be used instead.
    /// </summary>
    /// <remarks>Default: <c>*</c></remarks>
    [J("maxZoom")] public double? MaxZoom { get; set; }

    #endregion

    #region Animation Options

    #endregion

    //public List<TileLayer> TileLayers { get; set; } = [];

    /// <summary>
    /// Panes are DOM elements used to control the ordering of layers on the map.
    /// You can access panes with map.getPane or map.getPanes methods.
    /// New panes can be created with the map.createPane method.
    /// Every map has the following default panes that differ only in zIndex.
    /// </summary>
    public enum Panes
    {
        [J("tilePane")] TilePane,
        [J("overlayPane")] OverlayPane,
        [J("shadowPane")] ShadowPane,
        [J("markerPane")] MarkerPane,
        [J("tooltipPane")] TooltipPane,
        [J("popupPane")] PopupPane,
    }
}
