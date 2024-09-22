namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Encapsulates an encoded polyline.
/// </summary>
public class Polyline
{
    /// <summary>
    /// The string encoding of the polyline using the <see cref="https://developers.google.com/maps/documentation/utilities/polylinealgorithm">polyline encoding algorithm</see>.
    /// </summary>
    [J("encodedPolyline")][I(Condition = C.WhenWritingNull)] public string? EncodedPolyline { get; set; }
    /// <summary>
    /// Specifies a polyline using the <see cref="https://tools.ietf.org/html/rfc7946#section-3.1.4">GeoJSON LineString format</see>.
    /// </summary>
    [J("geoJsonLinestring")] public GeoJsonLinestring? GeoJsonLinestring { get; set; }
}
