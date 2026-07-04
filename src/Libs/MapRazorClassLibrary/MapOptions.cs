namespace Seedysoft.Libs.MapRazorClassLibrary;

public class MapOptions
{
    [J("center")] public Travel.Models.Location? Center { get; set; }

    [J("zoom")] public int? Zoom { get; set; }

    [J("minZoom")] public int? MinZoom { get; set; }

    [J("maxZoom")] public int? MaxZoom { get; set; }

    [J("zoomControl")] public bool? ZoomControl { get; set; } = true;

    [J("attributionControl")] public bool? AttributionControl { get; set; } = true;

    [J("scrollWheelZoom")] public bool? ScrollWheelZoom { get; set; } = true;

    [J("doubleClickZoom")] public bool? DoubleClickZoom { get; set; } = true;

    [J("dragging")] public bool? Dragging { get; set; } = true;

    [J("preferCanvas")] public bool? PreferCanvas { get; set; }

    public List<TileLayer> TileLayers { get; set; } = [];
    public LayersControlOptions? LayersControl { get; set; }
    public ScaleControlOptions? ScaleControl { get; set; }
}

public class TileLayer
{
    public required string Name { get; set; }
    public required string TyleUrl { get; set; }
    public string Attribution { get; set; } = string.Empty;
}

public enum ControlPosition
{
    [J("topleft")] TopLeft,
    [J("topright")] TopRight,
    [J("bottomleft")] BottomLeft,
    [J("bottomright")] BottomRight
}

public abstract class ControlOptions
{
    public ControlPosition? Position { get; set; }
}

public sealed class LayersControlOptions : ControlOptions
{

    /// <summary>
    /// If true, the control will be collapsed into an icon and expanded on mouse hover, touch, or keyboard activation.
    /// </summary>
    /// <remarks>Default is <see langword="true"/></remarks>
    public bool? Collapsed { get; set; }

    /// <summary>
    /// If true, the control will assign zIndexes in increasing order to all of its layers so that the order is preserved when switching them on/off.
    /// </summary>
    /// <remarks>Default is <see langword="true"/></remarks>
    public bool? AutoZIndex { get; set; }

    /// <summary>
    /// If true, the base layers in the control will be hidden when there is only one.
    /// </summary>
    /// <remarks>Default is <see langword="false"/></remarks>
    public bool? HideSingleBase { get; set; }

    /// <summary>
    /// Whether to sort the layers. When false, layers will keep the order in which they were added to the control.
    /// </summary>
    /// <remarks>Default is <see langword="false"/></remarks>
    public bool? SortLayers { get; set; }
}

public class ScaleControlOptions : ControlOptions
{
    /// <summary>
    /// Maximum width of the control in pixels. The width is set dynamically to show round values (e.g. 100, 200, 500).
    /// </summary>
    /// <remarks>Default is 100.</remarks>
    public int? MaxWidth { get; set; }

    /// <summary>
    /// Whether to show the imperial scale line (mi/ft).
    /// </summary>
    /// <remarks>Default is <see langword="true"/></remarks>
    public bool? Imperial { get; set; }

    /// <summary>
    /// Whether to show the metric scale line (m/km).
    /// </summary>
    /// <remarks>Default is <see langword="true"/></remarks>
    public bool? Metric { get; set; }

    /// <summary>
    /// If true, the control is updated on moveend, otherwise it's always up-to-date (updated on move).
    /// </summary>
    /// <remarks>Default is <see langword="false"/></remarks>
    public bool? UpdateWhenIdle { get; set; }
}
