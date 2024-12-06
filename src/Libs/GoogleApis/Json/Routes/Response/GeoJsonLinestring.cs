namespace Seedysoft.Libs.GoogleApis.Json.Routes.Response;

/// <summary>
/// 
/// </summary>
public class GeoJsonLinestring
{
    /// <summary>
    /// "LineString".
    /// </summary>
    [J("type")] public required string Type { get; set; }
    /// <summary>
    /// For type "LineString", the "coordinates" member is an array of two or more positions.
    /// </summary>
    [J("coordinates")] public required double[][] Coordinates { get; set; }
}
