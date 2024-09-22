namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// A set of values that specify the quality of the polyline.
/// </summary>
public enum PolylineQuality
{
    /// <summary>
    /// Specifies a high-quality polyline - which is composed using more points than <see cref="OVERVIEW"/>, at the cost of increased response size. Use this value when you need more precision.
    /// </summary>
    HIGH_QUALITY,
    /// <summary>
    /// Specifies an overview polyline - which is composed using a small number of points.Use this value when displaying an overview of the route. Using this option has a lower request latency compared to using the <see cref="HIGH_QUALITY"/> option.
    /// </summary>
    OVERVIEW,
    /// <summary>
    /// No polyline quality preference specified. Defaults to <see cref="OVERVIEW"/>.
    /// </summary>
    POLYLINE_QUALITY_UNSPECIFIED = OVERVIEW,
}
