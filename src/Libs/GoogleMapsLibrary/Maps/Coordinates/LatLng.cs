using System.Diagnostics;

namespace GoogleMapsLibrary.Maps.Coordinates;

/// <summary>
/// A <see cref="LatLng"/> is a point in geographical coordinates: latitude and longitude.
/// Latitude ranges between -90 and 90 degrees, inclusive.Values above or below this range will be clamped to the range[-90, 90].
/// This means that if the value specified is less than -90, it will be set to -90.
/// And if the value is greater than 90, it will be set to 90.
/// Longitude ranges between -180 and 180 degrees, inclusive.
/// Values above or below this range will be wrapped so that they fall within the range.
/// For example, a value of -190 will be converted to 170.
/// A value of 190 will be converted to -170.
/// This reflects the fact that longitudes wrap around the globe.
/// Although the default map projection associates longitude with the x-coordinate of the map, and latitude with the y-coordinate, the latitude coordinate is always written first, followed by the longitude.
/// Notice that you cannot modify the coordinates of a LatLng.
/// If you want to compute another point, you have to create a new one.
/// Most methods that accept LatLng objects also accept a LatLngLiteral object.
/// </summary>
/// <remarks>https://developers.google.com/maps/documentation/javascript/reference/coordinates#LatLng</remarks>
[DebuggerDisplay("{Lat}, {Lng}")]
public class LatLng
{
    private double lat;
    /// <summary>
    /// Returns the latitude in degrees.
    /// </summary>
    [JsonPropertyName("lat")]
    public double Lat
    {
        get => lat;
        init => lat = value switch
        {
            < -90d => -90d,
            > 90d => 90d,
            _ => lat,
        };
    }

    private double lng;
    /// <summary>
    /// Returns the longitude in degrees.
    /// </summary>
    [JsonPropertyName("lng")]
    public double Lng
    {
        get => lng;
        init => lng = value switch
        {
            < -180d => -180d,
            > 180d => 180d,
            _ => lng,
        };
    }

    public LatLng() { }
    /// <summary>
    /// Creates a <see cref="LatLng"> object representing a geographic point.
    /// Latitude is specified in degrees within the range [-90, 90].
    /// Longitude is specified in degrees within the range [-180, 180).
    /// Set noClampNoWrap to true to enable values outside of this range.
    /// Note the ordering of latitude and longitude.
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lng"></param>
    public LatLng(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }
    public LatLng(decimal lat, decimal lng) : this(Convert.ToDouble(lat), Convert.ToDouble(lng)) { }
    public LatLng(LatLng latLng) : this(latLng.Lat, latLng.Lng) { }
}
