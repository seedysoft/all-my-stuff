using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

public enum TransitRoutingPreference
{
    /// <summary>
    /// No TransitRoutingPreference.
    /// </summary>
    [EnumMember(Value = "TRANSIT_ROUTING_PREFERENCE_UNSPECIFIED")]
    TransitRoutingPreferenceUnspecified = 0,

    /// <summary>
    /// Indicates that the calculated route should prefer limited amounts of walking.
    /// </summary>
    [EnumMember(Value = "LESS_WALKING")]
    LessWalking = 1,

    /// <summary>
    /// Indicates that the calculated route should prefer a limited number of transfers.
    /// </summary>
    [EnumMember(Value = "FEWER_TRANSFERS")]
    FewerTransfers = 2,
}
