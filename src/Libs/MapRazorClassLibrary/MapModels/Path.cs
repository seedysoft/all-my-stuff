namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// An abstract class that contains options and constants shared between vector overlays (Polygon, Polyline, Circle).
/// Do not use it directly.
/// Extends <see cref="Layer"/>.
/// </summary>
public abstract record class Path : InteractiveLayer
{
    protected Path() : base() { }

    /// <summary>
    /// Whether to draw stroke along the path. Set it to <c>false</c> to disable borders on polygons or circles.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("stroke")] public bool? Stroke { get; set; } = true;

    /// <summary>
    /// Stroke color.
    /// </summary>
    /// <remarks>Default: #3388ff</remarks>
    [J("color")] public string? Color { get; set; } = "#3388ff";

    /// <summary>
    /// Stroke width in pixels.
    /// </summary>
    /// <remarks>Default: 3</remarks>
    [J("weight")] public double? Weight { get; set; } = 3;

    /// <summary>
    /// Stroke opacity
    /// </summary>
    /// <value>butt | round | square</value>
    /// <remarks>Default: 1.0</remarks>
    [J("opacity")] public double? Opacity { get; set; } = 1.0;

    /// <summary>
    /// A string that defines <see href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-linecap">shape to be used at the end</see> of the stroke.
    /// </summary>
    /// <remarks>Default: 'round'</remarks>
    [J("lineCap")] public string? LineCap { get; set; } = "round";

    /// <summary>
    /// A string that defines <see href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-linejoin">shape to be used at the corners</see> of the stroke.
    /// </summary>
    /// <value>butt | round | square</value>
    /// <remarks>Default: 'round'</remarks>
    [J("lineJoin")] public string? LineJoin { get; set; } = "round";

    /// <summary>
    /// A string that defines the stroke dash pattern.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("dashArray")] public string? DashArray { get; set; } = null;

    /// <summary>
    /// A string that defines the distance into the dash pattern to start the dash.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("dashOffset")] public string? DashOffset { get; set; } = null;

    /// <summary>
    /// Whether to fill the path with color.
    /// Set it to false to disable filling on polygons or circles.
    /// </summary>
    /// <remarks>Default: depends</remarks>
    [J("fill")] public bool? Fill { get; set; }

    /// <summary>
    /// Fill color.
    /// Defaults to the value of the <see cref="Color"/> option.
    /// </summary>
    /// <remarks>Default: *</remarks>
    [J("fillColor")] public string? FillColor { get; set; }

    /// <summary>
    /// Fill opacity.
    /// </summary>
    /// <remarks>Default: 0.2</remarks>
    [J("fillOpacity")] public double? FillOpacity { get; set; } = 0.2;

    /// <summary>
    /// A string that defines how the inside of a shape is determined.
    /// </summary>
    /// <remarks>Default: 'evenodd'</remarks>
    [J("fillRule")] public string? FillRule { get; set; } = "evenodd";

    /// <summary>
    /// Use this specific instance of <see cref="Renderer"/> for this path.
    /// Takes precedence over the map's default renderer.
    /// If set, it will override the pane option of the path.
    /// </summary>
    [J("renderer")] public Renderer? Renderer { get; set; }

    /// <summary>
    /// Custom class name set on an element.
    /// Only for SVG renderer.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("className")] public string? ClassName { get; set; } = null;
}
