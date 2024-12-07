using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// A set of values that specify the quality of the polyline.
/// </summary>
public enum PolylineQuality
{
    /// <summary>
    /// No polyline quality preference specified.
    /// Defaults to <see cref="Overview"/>.
    /// </summary>
    [EnumMember(Value = "POLYLINE_QUALITY_UNSPECIFIED")]
    PolylineQualityUnspecified,

    /// <summary>
    /// Specifies a high-quality polyline - which is composed using more points than <see cref="Overview"/>, at the cost of increased response size.
    /// Use this value when you need more precision.
    /// </summary>
    [EnumMember(Value = "HIGH_QUALITY")]
    HighQuality,

    /// <summary>
    /// Specifies an overview polyline - which is composed using a small number of points.
    /// Use this value when displaying an overview of the route.
    /// Using this option has a lower request latency compared to using the <see cref="HighQuality"/> option.
    /// </summary>
    [EnumMember(Value = "OVERVIEW")]
    Overview,
}
