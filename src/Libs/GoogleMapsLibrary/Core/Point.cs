namespace GoogleMapsLibrary.Core;

/// <summary>
/// A Point geometry contains a single LatLng.
/// </summary>
/// <see href="https://developers.google.com/maps/documentation/javascript/reference/coordinates#Point"/>
//public class Point : Geometry
//{
//    private readonly LatLngLiteral _latLng;

//    public Point(LatLngLiteral latLng)
//    {
//        _latLng = latLng;
//    }

//    public override IEnumerator<LatLngLiteral> GetEnumerator()
//    {
//        yield return _latLng;
//    }

//    /// <summary>
//    /// Returns the contained LatLng.
//    /// </summary>
//    /// <returns></returns>
//    public LatLngLiteral Get()
//    {
//        return _latLng;
//    }
//}
public class Point
{
    /// <summary>
    /// The X coordinate
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// The Y coordinate
    /// </summary>
    public double Y { get; set; }
}
