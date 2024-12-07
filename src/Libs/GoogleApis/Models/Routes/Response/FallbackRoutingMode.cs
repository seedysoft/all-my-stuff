using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Actual routing mode used for returned fallback response.
/// </summary>
public enum FallbackRoutingMode
{
    /// <summary>
    /// Not used.
    /// </summary>
    [EnumMember(Value = "FALLBACK_ROUTING_MODE_UNSPECIFIED")]
    FallbackRoutingModeUnspecified,

    /// <summary>
    /// Indicates the <see cref="Request.RoutingPreference.TRAFFIC_UNAWARE"/> <see cref="Request.RoutingPreference"/> was used to compute the response.
    /// </summary>
    [EnumMember(Value = "FALLBACK_TRAFFIC_UNAWARE")]
    FallbackTrafficUnaware,

    /// <summary>
    /// Indicates the <see cref="Request.RoutingPreference.TRAFFIC_AWARE"/> <see cref="Request.RoutingPreference"/> was used to compute the response.
    /// </summary>
    [EnumMember(Value = "FALLBACK_TRAFFIC_AWARE")]
    FallbackTrafficAware,
}
