namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// A set of values used to specify the mode of transit.
/// </summary>
public enum TransitTravelMode
{
    /// <summary>
    /// No transit travel mode specified.
    /// </summary>
    TRANSIT_TRAVEL_MODE_UNSPECIFIED,
    /// <summary>
    /// Travel by bus.
    /// </summary>
    BUS,
    /// <summary>
    /// Travel by subway.
    /// </summary>
    SUBWAY,
    /// <summary>
    /// Travel by train.
    /// </summary>
    TRAIN,
    /// <summary>
    /// Travel by light rail or tram.
    /// </summary>
    LIGHT_RAIL,
    /// <summary>
    /// Travel by rail. This is equivalent to a combination of <see cref="SUBWAY"/>, <see cref="TRAIN"/>, and <see cref="LIGHT_RAIL"/>.
    /// </summary>
    RAIL,
}
