using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// Specifies the preferred type of polyline to be returned.
/// </summary>
public enum PolylineEncoding
{
    /// <summary>
    /// No polyline type preference specified.
    /// Defaults to <see cref="EncodedPolyline"/>.
    /// </summary>
    [EnumMember(Value = "POLYLINE_ENCODING_UNSPECIFIED")]
    PolylineEncodingUnspecified,

    /// <summary>
    /// Specifies a polyline encoded using the <see href="https://developers.google.com/maps/documentation/utilities/polylinealgorithm">polyline encoding algorithm</see>.
    /// </summary>
    [EnumMember(Value = "ENCODED_POLYLINE")]
    EncodedPolyline,

    /// <summary>
    /// Specifies a polyline using the <see href="https://tools.ietf.org/html/rfc7946#section-3.1.4">GeoJSON LineString format</see>.
    /// </summary>
    [EnumMember(Value = "GEO_JSON_LINESTRING")]
    GeoJsonLinestring,
}
