namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public record class Marker
{
    [I(Condition = C.WhenWritingNull)] public string? Content { get; set; }

    public PinElement? PinElement { get; set; }

    public MarkerPosition? Position { get; set; }

    [I(Condition = C.WhenWritingNull)] public string? Title { get; set; }
}
