using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// A supported reference route on the ComputeRoutesRequest.
/// </summary>
public enum ReferenceRoute
{
    /// <summary>
    /// Not used. Requests containing this value fail.
    /// </summary>
    [EnumMember(Value = "REFERENCE_ROUTE_UNSPECIFIED")]
    ReferenceRouteUnspecified,

    /// <summary>
    /// Fuel efficient route.
    /// </summary>
    [EnumMember(Value = "FUEL_EFFICIENT")]
    FuelEfficient,

    /// <summary>
    /// Route with shorter travel distance.
    /// This is an experimental feature.
    /// For DRIVE requests, this feature prioritizes shorter distance over driving comfort.
    /// For example, it may prefer local roads instead of highways, take dirt roads, cut through parking lots, etc.
    /// This feature does not return any maneuvers that Google Maps knows to be illegal.
    /// For BICYCLE and TWO_WHEELER requests, this feature returns routes similar to those returned when you don't specify requestedReferenceRoutes.
    /// This feature is not compatible with any other travel modes, via intermediate waypoints, or optimizeWaypointOrder; such requests will fail.
    /// However, you can use it with any routingPreference.
    /// </summary>
    [EnumMember(Value = "SHORTER_DISTANCE")]
    ShorterDistance,
}
