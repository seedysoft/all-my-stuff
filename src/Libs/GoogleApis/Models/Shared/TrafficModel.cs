using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Shared;

/// <summary>
/// The assumptions to use when predicting duration in traffic. 
/// Specified as part of a DirectionsRequest or DistanceMatrixRequest. 
/// </summary>
public enum TrafficModel
{
    /// <summary>
    /// Use historical traffic data to best estimate the time spent in traffic.
    /// </summary>
    [EnumMember(Value = "BEST_GUESS")]
    BestGuess,

    /// <summary>
    /// Use historical traffic data to make an optimistic estimate of what the duration in traffic will be.
    /// </summary>
    [EnumMember(Value = "OPTIMISTIC")]
    Optimistic,

    /// <summary>
    /// Use historical traffic data to make a pessimistic estimate of what the duration in traffic will be.
    /// </summary>
    [EnumMember(Value = "PESSIMISTIC")]
    Pessimistic
}
