namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public record class PinElement
{
    [I(Condition = C.WhenWritingNull)] public string? Background { get; set; }

    [I(Condition = C.WhenWritingNull)] public string? BorderColor { get; set; }

    [I(Condition = C.WhenWritingNull)] public object? Glyph { get; set; }

    [I(Condition = C.WhenWritingNull)] public string? GlyphColor { get; set; }

    public double Scale { get; set; } = 1.0;

    public bool UseIconFonts { get; set; }
}
