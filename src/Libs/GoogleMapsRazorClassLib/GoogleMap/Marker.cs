namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public record class Marker
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [I(Condition = C.WhenWritingNull)]
    public string? Content { get; set; }

    public PinElement? PinElement { get; set; }

    public GoogleApis.Models.Shared.LatLngLiteral? Position { get; set; }

    [I(Condition = C.WhenWritingNull)]
    public string? Title { get; set; }

    [I(Condition = C.WhenWritingNull), J("zIndex")]
    public int? ZIndex { get; set; }
}
