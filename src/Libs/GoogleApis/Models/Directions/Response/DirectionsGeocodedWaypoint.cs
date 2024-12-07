//namespace Seedysoft.Libs.GoogleApis.Models.Directions.Response;

///// <summary>
///// Details about the locations used as waypoints.
///// Only populated for address waypoints.
///// Includes details about the geocoding results for the purposes of determining what the address was geocoded to.
///// </summary>
//public class DirectionsGeocodedWaypoint
//{
//    /// <summary>
//    /// Indicates the status code resulting from the geocoding operation.
//    /// This field may contain the following values.
//    /// The allowed values include: OK, and ZERO_RESULTS
//    /// </summary>
//    [J("geocoder_status"), I(Condition = C.WhenWritingNull)]
//    public string? GeocoderStatus { get; set; }

//    /// <summary>
//    /// Indicates that the geocoder did not return an exact match for the original request, though it was able to match part of the requested address.
//    /// You may wish to examine the original request for misspellings and/or an incomplete address.
//    /// Partial matches most often occur for street addresses that do not exist within the locality you pass in the request.
//    /// Partial matches may also be returned when a request matches two or more locations in the same locality.
//    /// For example, "21 Henr St, Bristol, UK" will return a partial match for both Henry Street and Henrietta Street.
//    /// Note that if a request includes a misspelled address component, the geocoding service may suggest an alternative address.
//    /// Suggestions triggered in this way will also be marked as a partial match.
//    /// </summary>
//    [J("partial_match")]
//    public bool PartialMatch { get; set; }

//    /// <summary>
//    /// A unique identifier that can be used with other Google APIs.
//    /// See the <see href="https://developers.google.com/maps/documentation/places/web-service/place-id">Place ID overview</see>.
//    /// </summary>
//    [J("place_id"), I(Condition = C.WhenWritingNull)]
//    public string? PlaceId { get; set; }

//    /// <summary>
//    /// <see href="https://developers.google.com/maps/documentation/directions/get-directions#DirectionsGeocodedWaypoint-types">
//    /// </summary>
//    [J("types"), I(Condition = C.WhenWritingNull)]
//    public string[]? Types { get; set; }
//}
