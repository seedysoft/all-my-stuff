using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// A set of values used to specify the mode of transit.
/// </summary>
public enum TransitTravelMode
{
    /// <summary>
    /// No transit travel mode specified.
    /// </summary>
    [EnumMember(Value = "TRANSIT_TRAVEL_MODE_UNSPECIFIED")]
    RoutingPreferenceUnspecified,

    /// <summary>
    /// Travel by bus.
    /// </summary>
    [EnumMember(Value = "BUS")]
    Bus,

    /// <summary>
    /// Travel by subway.
    /// </summary>
    [EnumMember(Value = "SUBWAY")]
    Subway,

    /// <summary>
    /// Travel by train.
    /// </summary>
    [EnumMember(Value = "TRAIN")]
    Train,

    /// <summary>
    /// Travel by light rail or tram.
    /// </summary>
    [EnumMember(Value = "LIGHT_RAIL")]
    LightRail,

    /// <summary>
    /// Travel by rail.
    /// This is equivalent to a combination of <see cref="Subway"/>, <see cref="Train"/>, and <see cref="LightRail"/>.
    /// </summary>
    [EnumMember(Value = "RAIL")]
    Rail,
}
