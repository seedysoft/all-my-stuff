﻿namespace Seedysoft.Libs.GoogleApis.Models.Directions.Response;

/// <summary>
/// A single leg consisting of a set of steps in a DirectionsResult. 
/// Some fields in the leg may not be returned for all requests. 
/// Note that though this result is "JSON-like," it is not strictly JSON, as it directly and indirectly includes LatLng objects..
/// </summary>
public class DirectionsLeg
{
    ///// <summary>
    ///// Contains the human-readable address (typically a street address) from reverse geocoding the end_location of this leg.
    ///// This content is meant to be read as-is.
    ///// Do not programmatically parse the formatted address.
    ///// </summary>
    //[J("end_address")]
    //public string? EndAddress { get; init; }

    ///// <summary>
    ///// An estimated arrival time for this leg. Only applicable for TRANSIT requests.
    ///// </summary>
    //public TimeZoneTextValueObject? ArrivalTime { get; init; }

    ///// <summary>
    ///// An estimated departure time for this leg. Only applicable for TRANSIT requests.
    ///// </summary>
    //public TimeZoneTextValueObject? DepartureTime { get; init; }

    /// <summary>
    /// The total distance covered by this leg. This property may be undefined as the distance may be unknown.
    /// </summary>
    public Distance? Distance { get; init; }

    /// <summary>
    /// The total duration of this leg. This property may be undefined as the duration may be unknown.
    /// </summary>
    public Duration? Duration { get; init; }

    ///// <summary>
    ///// The total duration of this leg, taking into account the traffic conditions indicated by the trafficModel property. 
    ///// This property may be undefined as the duration may be unknown. 
    ///// Only available to Premium Plan customers when drivingOptions is defined when making the request.
    ///// </summary>

    //[J("duration_in_traffic")]
    //public Duration? DurationInTraffic { get; init; }

    ///// <summary>
    ///// The DirectionsService calculates directions between locations by using the nearest transportation option (usually a road) at the start and end locations. 
    ///// end_location indicates the actual geocoded destination, which may be different than the end_location of the last step if, for example, the road is not near the destination of this leg.
    ///// </summary>
    //[J("end_location")]
    //public Models.Shared.LatLngLiteral? EndLocation { get; init; }

    ///// <summary>
    ///// The address of the origin of this leg.
    ///// </summary>
    //[J("start_address")]
    //public string? StartAddress { get; init; }

    ///// <summary>
    ///// The DirectionsService calculates directions between locations by using the nearest transportation option (usually a road) at the start and end locations. 
    ///// start_location indicates the actual geocoded origin, which may be different than the start_location of the first step if, for example, the road is not near the origin of this leg.
    ///// </summary>
    //[J("start_location")]
    //public Models.Shared.LatLngLiteral? StartLocation { get; init; }

    /// <summary>
    /// An array of DirectionsSteps, each of which contains information about the individual steps in this leg.
    /// </summary>
    public IEnumerable<DirectionsStep>? Steps { get; init; }

    ///// <summary>
    ///// An array of non-stopover waypoints along this leg, which were specified in the original request.
    ///// Deprecated in alternative routes.Version 3.27 will be the last version of the API that adds extra via_waypoints in alternative routes.
    ///// When using the Directions Service to implement draggable directions, it is recommended to disable dragging of alternative routes.
    ///// Only the main route should be draggable.Users can drag the main route until it matches an alternative route.
    ///// </summary>
    //[J("via_waypoints")]
    //public IEnumerable<Models.Shared.LatLngLiteral>? ViaWaypoints { get; init; }
}
