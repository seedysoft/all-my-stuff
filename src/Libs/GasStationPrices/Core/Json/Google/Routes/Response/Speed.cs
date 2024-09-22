namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// The classification of polyline speed based on traffic data.
/// </summary>
public enum Speed
{
    /// <summary>
    /// Default value. This value is unused.
    /// </summary>
    SPEED_UNSPECIFIED,
    /// <summary>
    /// Normal speed, no slowdown is detected.
    /// </summary>
    NORMAL,
    /// <summary>
    /// Slowdown detected, but no traffic jam formed.
    /// </summary>
    SLOW,
    /// <summary>
    /// Traffic jam detected.
    /// </summary>
    TRAFFIC_JAM,
}
