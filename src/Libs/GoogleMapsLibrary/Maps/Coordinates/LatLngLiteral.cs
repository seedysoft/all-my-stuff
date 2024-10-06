using System.Diagnostics;

namespace GoogleMapsLibrary.Maps.Coordinates;

/// <summary>
/// Object literals are accepted in place of LatLng objects, as a convenience, in many places. 
/// These are converted to LatLng objects when the Maps API encounters them.
/// </summary>
[DebuggerDisplay("{Lat}, {Lng}")]
public class LatLngLiteral : LatLng
{
    public LatLngLiteral() : base() { }
    public LatLngLiteral(double lat, double lng) : base(lat, lng) { }
    public LatLngLiteral(decimal lat, decimal lng) : base(lat, lng) { }
    public LatLngLiteral(LatLng latLng) : base(latLng) { }
    public LatLngLiteral(LatLngLiteral latLngLiteral) : base(latLngLiteral) { }
}
