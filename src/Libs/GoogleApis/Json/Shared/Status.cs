using Seedysoft.Libs.GoogleApis.Json.Directions.Request;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Seedysoft.Libs.GoogleApis.Json.Shared;

/// <summary>
/// The status returned by the <see cref="Service"/> on the completion of a call to <see cref="Service.RouteAsync"/>().
/// Specify these by value, or by using the constant's name.
/// For example, 'OK' or google.maps.DirectionsStatus.OK.
/// </summary>
[K(typeof(JsonStringEnumConverter<Status>))]
public enum Status
{
    /// <summary>
    /// The <see cref="Request"/> provided was invalid.
    /// </summary>,
    [EnumMember(Value = "INVALID_REQUEST")]
    invalidRequest,

    /// <summary>
    /// Too many DirectionsWaypoints were provided in the <see cref="Request"/>.
    /// See the developer's guide for the maximum number of waypoints allowed.
    /// </summary>
    [EnumMember(Value = "MAX_WAYPOINTS_EXCEEDED")]
    maxWaypointsExceeded,

    /// <summary>
    /// At least one of the origin, destination, or waypoints could not be geocoded.
    /// </summary>
    [EnumMember(Value = "NOT_FOUND")]
    notFound,

    /// <summary>
    /// The response contains a valid DirectionsResult.
    /// </summary>
    [EnumMember(Value = "OK")]
    ok,

    /// <summary>
    /// The webpage has gone over the requests limit in too short a period of time.
    /// </summary>
    [EnumMember(Value = "OVER_QUERY_LIMIT")]
    overQueryLimit,

    /// <summary>
    /// The webpage is not allowed to use the directions service.
    /// </summary>
    [EnumMember(Value = "REQUEST_DENIED")]
    requestDenied,

    /// <summary>
    /// A directions request could not be processed due to a server error.
    /// The request may succeed if you try again.
    /// </summary>
    [EnumMember(Value = "UNKNOWN_ERROR")]
    unknownError,

    /// <summary>
    /// No route could be found between the origin and destination.
    /// </summary>
    [EnumMember(Value = "ZERO_RESULTS")]
    zeroResults,
}
