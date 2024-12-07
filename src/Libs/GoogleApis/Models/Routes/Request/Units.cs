using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// The valid unit systems that can be specified in a DirectionsRequest.
/// </summary>
public enum Units
{
    /// <summary>
    /// Specifies that distances in the DirectionsResult should be expressed in imperial units.
    /// </summary>
    [EnumMember(Value = "IMPERIAL")]
    Imperial,

    /// <summary>
    /// Specifies that distances in the DirectionsResult should be expressed in metric units.
    /// </summary>
    [EnumMember(Value = "METRIC")]
    Metric,
}
