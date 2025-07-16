namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public record class PinElement
{
    /// <summary>
    /// The background color of the pin shape. Supports any CSS color value.
    /// </summary>
    [I(Condition = C.WhenWritingNull)]
    public string? Background { get; set; }

    /// <summary>
    /// The border color of the pin shape. Supports any CSS color value.
    /// </summary>
    [I(Condition = C.WhenWritingNull)]
    public string? BorderColor { get; set; }

    /// <summary>
    /// The DOM element displayed in the pin.
    /// </summary>
    [I(Condition = C.WhenWritingNull)]
    public object? Glyph { get; set; }

    /// <summary>
    /// The color of the glyph. Supports any CSS color value.
    /// </summary>
    [I(Condition = C.WhenWritingNull)]
    public string? GlyphColor { get; set; }

    /// <summary>
    /// The scale of the pin.
    /// </summary>
    public double Scale { get; set; } = 1.0;
}
