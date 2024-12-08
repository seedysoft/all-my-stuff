namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Details about the locations used as waypoints.
/// Only populated for address waypoints.
/// Includes details about the geocoding results for the purposes of determining what the address was geocoded to.
/// </summary>
public record GeocodedWaypoint
{
    /// <summary>
    /// Indicates the status code resulting from the geocoding operation.
    /// </summary>
    [J("geocoderStatus"), I(Condition = C.WhenWritingNull)]
    public Status? GeocoderStatus { get; init; }

    /// <summary>
    /// The type(s) of the result, in the form of zero or more type tags.
    /// Supported types: <see href="https://developers.google.com/maps/documentation/geocoding/requests-geocoding#Types">Address types and address component types</see>.
    /// </summary>
    [J("type"), I(Condition = C.WhenWritingNull)]
    public string[]? Type { get; init; }

    /// <summary>
    /// Indicates that the geocoder did not return an exact match for the original request, though it was able to match part of the requested address.
    /// You may wish to examine the original request for misspellings and/or an incomplete address.
    /// </summary>
    [J("partialMatch")]
    public bool PartialMatch { get; init; }

    /// <summary>
    /// The place ID for this result.
    /// </summary>
    [J("placeId"), I(Condition = C.WhenWritingNull)]
    public string? PlaceId { get; init; }

    /// <summary>
    /// The index of the corresponding intermediate waypoint in the request.
    /// Only populated if the corresponding waypoint is an intermediate waypoint.
    /// </summary>
    [J("intermediateWaypointRequestIndex"), I(Condition = C.WhenWritingNull)]
    public long? IntermediateWaypointRequestIndex { get; init; }
}
