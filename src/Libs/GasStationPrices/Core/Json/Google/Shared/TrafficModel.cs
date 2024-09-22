namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// Specifies the assumptions to use when calculating time in traffic. This setting affects the value returned in the <see cref="Route.Duration"/> field in the response, which contains the predicted time in traffic based on historical averages.
/// </summary>
public enum TrafficModel
{
    /// <summary>
    /// Indicates that the returned duration should be the best estimate of travel time given what is known about both historical traffic conditions and live traffic. Live traffic becomes more important the closer the <see cref="Body.DepartureTime"/> is to now.
    /// </summary>
    BEST_GUESS,
    /// <summary>
    /// Indicates that the returned duration should be longer than the actual travel time on most days, though occasional days with particularly bad traffic conditions may exceed this value.
    /// </summary>
    PESSIMISTIC,
    /// <summary>
    /// Indicates that the returned duration should be shorter than the actual travel time on most days, though occasional days with particularly good traffic conditions may be faster than this value.
    /// </summary>
    OPTIMISTIC,
    /// <summary>
    /// Unused. If specified, will default to BEST_GUESS.
    /// </summary>
    TRAFFIC_MODEL_UNSPECIFIED = BEST_GUESS,
}
