namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Specifies a polyline using the <see href="https://tools.ietf.org/html/rfc7946#section-3.1.4">GeoJSON LineString format</see>.
/// </summary>
public class GeoJsonLinestring
{
    /// <summary>
    /// "LineString".
    /// </summary>
    [J("type")]
    public required string Type { get; init; }

    /// <summary>
    /// For type "LineString", the "coordinates" member is an array of two or more positions.
    /// </summary>
    [J("coordinates")]
    public required double[][] Coordinates { get; init; }
}
