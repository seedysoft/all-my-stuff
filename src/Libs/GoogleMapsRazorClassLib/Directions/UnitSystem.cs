using System.Text.Json.Serialization;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib.Directions;

/// <summary>
/// The valid unit systems that can be specified in a DirectionsRequest.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<UnitSystem>))]
public enum UnitSystem
{
    /// <summary>
    /// Specifies that distances in the DirectionsResult should be expressed in imperial units.
    /// </summary>
    Imperial,

    /// <summary>
    /// Specifies that distances in the DirectionsResult should be expressed in metric units.
    /// </summary>
    Metric
}
