using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// Extra computations to perform while completing the request.
/// </summary>
public enum ExtraComputation
{
    /// <summary>
    /// Not used.
    /// Requests containing this value will fail.
    /// </summary>
    [EnumMember(Value = "EXTRA_COMPUTATION_UNSPECIFIED")]
    ExtraComputationUnspecified,

    /// <summary>
    /// Toll information for the route(s).
    /// </summary>
    [EnumMember(Value = "TOLLS")]
    Tolls,

    /// <summary>
    /// Estimated fuel consumption for the route(s).
    /// </summary>
    [EnumMember(Value = "FUEL_CONSUMPTION")]
    FuelConsumption,

    /// <summary>
    /// Traffic aware polylines for the route(s).
    /// </summary>
    [EnumMember(Value = "TRAFFIC_ON_POLYLINE")]
    TrafficOnPolyline,

    /// <summary>
    /// NavigationInstructions presented as a formatted HTML text string.
    /// This content is meant to be read as-is.
    /// This content is for display only.
    /// Do not programmatically parse it.
    /// </summary>
    [EnumMember(Value = "HTML_FORMATTED_NAVIGATION_INSTRUCTIONS")]
    HtmlFormattedNavigationInstructions,

    /// <summary>
    /// Flyover information for the route(s).
    /// The routes.polyline_details.flyover_info fieldmask must be specified to return this information.
    /// This data will only currently be populated for certain metros in India.
    /// This feature is experimental, and the SKU/charge is subject to change.
    /// </summary>
    [EnumMember(Value = "FLYOVER_INFO_ON_POLYLINE")]
    FlyoverInfoOnPolyline,

    /// <summary>
    /// Narrow road information for the route(s).
    /// The routes.polyline_details.narrow_road_info fieldmask must be specified to return this information.
    /// This data will only currently be populated for certain metros in India.
    /// This feature is experimental, and the SKU/charge is subject to change.
    /// </summary>
    [EnumMember(Value = "NARROW_ROAD_INFO_ON_POLYLINE")]
    NarrowRoadInfoOnPolyline,
}
