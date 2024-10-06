namespace GoogleMapsLibrary.Routes;

/// <summary>
/// The assumptions to use when predicting duration in traffic.
/// Specified as part of a DirectionsRequest or DistanceMatrixRequest.
/// Specify these by value, or by using the constant's name.
/// For example, 'bestguess' or google.maps.TrafficModel.BEST_GUESS.
/// </summary>
/// <see href="https://developers.google.com/maps/documentation/javascript/reference/directions#TrafficModel"/>
[JsonConverter(typeof(JsonStringEnumConverter))]
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
    [EnumMember(Value = "BEST_GUESS")]
    Pessimistic
}
