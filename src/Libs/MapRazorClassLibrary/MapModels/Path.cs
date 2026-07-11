namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public record Path : Layer
{
    [J("options")] public override PathOptions? Options { get; }

    public Path(PathOptions? pathOptions = default) : base(pathOptions) => Options = pathOptions;
}

public record PathOptions : LayerOptions
{
    /// <summary>
    /// Stroke color
    /// </summary>
    /// <remarks>Default: #3388ff</remarks>
    [J("color")] public string? Color { get; set; }

    /// <summary>
    /// Fill opacity.
    /// </summary>
    /// <remarks>Default: 0.2</remarks>
    [J("fillOpacity")] public double? FillOpacity { get; set; }

    /// <summary>
    /// A string that defines <see href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-linecap">shape to be used at the end</see> of the stroke.
    /// </summary>
    /// <remarks>Default: 'round'</remarks>
    [J("lineCap")] public string? LineCap { get; set; }

    /// <summary>
    /// A string that defines <see href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-linejoin">shape to be used at the corners</see> of the stroke.
    /// </summary>
    /// <value>butt | round | square</value>
    /// <remarks>Default: 'round'</remarks>
    [J("lineJoin")] public string? LineJoin { get; set; }

    /// <summary>
    /// Stroke opacity
    /// </summary>
    /// <value>butt | round | square</value>
    /// <remarks>Default: 1.0</remarks>
    [J("opacity")] public double? Opacity { get; set; }

    /// <summary>
    /// Stroke width in pixels
    /// </summary>
    /// <remarks>Default: 3</remarks>
    [J("weight")] public double? Weight { get; set; }
}
