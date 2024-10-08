﻿namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Contains <see cref="GeocodedWaypoint"/>s for origin, destination and intermediate waypoints. Only populated for address waypoints.
/// </summary>
public class GeocodingResults
{
    /// <summary>
    /// Origin geocoded waypoint.
    /// </summary>
    [J("origin")] public GeocodedWaypoint? Origin { get; set; }
    /// <summary>
    /// Destination geocoded waypoint.
    /// </summary>
    [J("destination")] public GeocodedWaypoint? Destination { get; set; }
    /// <summary>
    /// A list of intermediate geocoded waypoints each containing an index field that corresponds to the zero-based position of the waypoint in the order they were specified in the request.
    /// </summary>
    [J("intermediates")] public GeocodedWaypoint[]? Intermediates { get; set; }
}
