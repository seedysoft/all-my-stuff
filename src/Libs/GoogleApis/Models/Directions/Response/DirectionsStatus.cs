//using System.Runtime.Serialization;

//namespace Seedysoft.Libs.GoogleApis.Models.Directions.Response;

///// <summary>
///// The status returned by the <see cref="Service"/> on the completion of a call to <see cref="Service.RouteAsync"/>().
///// Specify these by value, or by using the constant's name.
///// For example, 'OK' or google.maps.DirectionsStatus.OK.
///// </summary>
//public enum DirectionsStatus
//{
//    /// <summary>
//    /// The provided request was invalid.
//    /// Common causes of this status include an invalid parameter or parameter value.
//    /// </summary>,
//    [EnumMember(Value = "INVALID_REQUEST")]
//    InvalidRequest,

//    /// <summary>
//    /// The requested route is too long and cannot be processed.
//    /// This error occurs when more complex directions are returned.
//    /// Try reducing the number of waypoints, turns, or instructions.
//    /// </summary>
//    [EnumMember(Value = "MAX_ROUTE_LENGTH_EXCEEDED")]
//    MaxRouteLengthExceeded,

//    /// <summary>
//    /// Too many waypoints were provided in the request.
//    /// For applications using the Directions API as a web service, or the directions service in the Maps JavaScript API, the maximum allowed number of waypoints is 25, plus the origin and destination.
//    /// </summary>
//    [EnumMember(Value = "MAX_WAYPOINTS_EXCEEDED")]
//    MaxWaypointsExceeded,

//    /// <summary>
//    /// At least one of the locations specified in the request's origin, destination, or waypoints could not be geocoded.
//    /// </summary>
//    [EnumMember(Value = "NOT_FOUND")]
//    NotFound,

//    /// <summary>
//    /// The response contains a valid DirectionsResult.
//    /// </summary>
//    [EnumMember(Value = "OK")]
//    Ok,

//    /// <summary>
//    /// A any of the following:
//    /// <list type="">
//    /// <item>The API key is missing or invalid.</item>
//    /// <item>Billing has not been enabled on your account.</item>
//    /// <item>A self-imposed usage cap has been exceeded.</item>
//    /// <item>The provided method of payment is no longer valid (for example, a credit card has expired).</item>
//    /// </list>
//    /// See the Maps FAQ to learn how to fix this.
//    /// </summary>
//    [EnumMember(Value = "OVER_DAILY_LIMIT")]
//    OverDailyLimit,

//    /// <summary>
//    /// The service denied use of the directions service by your application.
//    /// </summary>
//    [EnumMember(Value = "REQUEST_DENIED")]
//    RequestDenied,

//    /// <summary>
//    /// A directions request could not be processed due to a server error.
//    /// The request may succeed if you try again.
//    /// </summary>
//    [EnumMember(Value = "UNKNOWN_ERROR")]
//    UnknownError,

//    /// <summary>
//    /// No route could be found between the origin and destination.
//    /// </summary>
//    [EnumMember(Value = "ZERO_RESULTS")]
//    ZeroResults,
//}
