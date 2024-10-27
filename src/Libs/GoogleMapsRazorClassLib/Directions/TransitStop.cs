namespace Seedysoft.Libs.GoogleMapsRazorClassLib.Directions;

/// <summary>
/// Details about a transit stop or station.
/// </summary>
public class TransitStop
{
    /// <summary>
    /// The location of this stop.
    /// </summary>
    public LatLngLiteral? Location { get; set; }

    /// <summary>
    /// The name of this transit stop.
    /// </summary>
    public string? Name { get; set; }
}
