namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

public enum PolylineEncoding
{
    /// <summary>
    /// Specifies a polyline encoded using the <see cref="https://developers.google.com/maps/documentation/utilities/polylinealgorithm">polyline encoding algorithm</see>.
    /// </summary>
    ENCODED_POLYLINE,
    /// <summary>
    /// Specifies a polyline using the <see cref="https://tools.ietf.org/html/rfc7946#section-3.1.4">GeoJSON LineString format</see>.
    /// </summary>
    GEO_JSON_LINESTRING,
    /// <summary>
    /// No polyline type preference specified. Defaults to <see cref="ENCODED_POLYLINE"/>.
    /// </summary>
    POLYLINE_ENCODING_UNSPECIFIED = ENCODED_POLYLINE,
}
