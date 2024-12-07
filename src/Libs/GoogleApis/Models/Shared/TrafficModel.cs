using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Shared;

/// <summary>
/// Specifies the assumptions to use when calculating time in traffic.
/// This setting affects the value returned in the duration field in the response, which contains the predicted time in traffic based on historical averages.
/// </summary>
public enum TrafficModel
{
    /// <summary>
    /// Unused.
    /// If specified, will default to <see cref="BestGuess"/>.
    /// </summary>
    [EnumMember(Value = "TRAFFIC_MODEL_UNSPECIFIED")]
    TrafficModelUnspecified,

    /// <summary>
    /// Indicates that the returned duration should be the best estimate of travel time given what is known about both historical traffic conditions and live traffic.
    /// Live traffic becomes more important the closer the departureTime is to now.
    /// </summary>
    [EnumMember(Value = "BEST_GUESS")]
    BestGuess,

    /// <summary>
    /// Indicates that the returned duration should be longer than the actual travel time on most days, though occasional days with particularly bad traffic conditions may exceed this value.
    /// </summary>
    [EnumMember(Value = "PESSIMISTIC")]
    Pessimistic,

    /// <summary>
    /// Indicates that the returned duration should be shorter than the actual travel time on most days, though occasional days with particularly good traffic conditions may be faster than this value.
    /// </summary>
    [EnumMember(Value = "OPTIMISTIC")]
    Optimistic,
}
