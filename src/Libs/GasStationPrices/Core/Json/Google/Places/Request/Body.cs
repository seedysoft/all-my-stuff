namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Request;

/// <summary>
/// The Autocomplete (New) service is a web service that returns place predictions and query predictions in response to an HTTP request.
/// In the request, specify a text search string and geographic bounds that controls the search area.
/// The Autocomplete(New) service can match on full words and substrings of the input, resolving place names, addresses, and plus codes.
/// Applications can therefore send queries as the user types, to provide on-the-fly place and query predictions.
/// The response from the Autocomplete(New) API can contain two types of predictions:
/// Place predictions: Places, such as businesses, addresses and points of interest, based on the specified input text string and search area.
/// Place predictions are returned by default.
/// Query predictions: Query strings matching the input text string and search area.
/// Query predictions are not returned by default.
/// Use the includeQueryPredictions request parameter to add query predictions to the response.
/// For example, you call the API using as input a string that contains a partial user input, "Sicilian piz", with the search area limited to San Francisco, CA.
/// The response then contains a list of place predictions that match the search string and search area, such as the restaurant named "Sicilian Pizza Kitchen", along with details about the place.
/// </summary>
public class Body
{
    /// <summary>
    /// The text string on which to search. Specify full words and substrings, place names, addresses, and plus codes.
    /// The Autocomplete (New) service returns candidate matches based on this string and orders results based on their perceived relevance.
    /// </summary>
    [J("input")] public required string Input { get; set; }

    /// <summary>
    /// Specifies an area to search.
    /// This location serves as a bias which means results around the specified location can be returned, including results outside the specified area.
    /// </summary>
    [J("locationbias")][I(Condition = C.WhenWritingNull)] public Location? LocationBias { get; set; }

    /// <summary>
    /// Specifies an area to search.
    /// Results outside the specified area are not returned.
    /// </summary>
    [J("locationrestriction")][I(Condition = C.WhenWritingNull)] public Location? LocationRestriction { get; set; }

    /// <summary>
    /// A place can only have a single primary type from types listed in Table A or Table B.
    /// For example, the primary type might be "mexican_restaurant" or "steak_house".
    /// By default, the API returns all places based on the input parameter, regardless of the primary type value associated with the place.
    /// Restrict results to be of a certain primary type or primary types by passing the includedPrimaryTypes parameter.
    /// Use this parameter to specify up to five type values from Table A or Table B.
    /// A place must match one of the specified primary type values to be included in the response.
    /// This parameter may also include, instead, one of (regions) or (cities).
    /// The (regions) type collection filters for areas or divisions, such as neighborhoods and postal codes.
    /// The (cities) type collection filters for places that Google identifies as a city.
    /// The request is rejected with an INVALID_REQUEST error if:<br/>
    /// More than five types are specified.<br/>
    /// Any type is specified in addition to (cities) or (regions).<br/>
    /// Any unrecognized types are specified.<br/>
    /// Note: The includedPrimaryTypes parameter only works on the primary type of the place, not all types associated with the place.
    /// Although every place has a primary type, not every primary type is supported by Places API (New).
    /// Supported types include those listed in Table A or Table B.
    /// </summary>
    [J("includedPrimaryTypes")][I(Condition = C.WhenWritingNull)] public string[]? IncludedPrimaryTypes { get; set; }

    /// <summary>
    /// Only include results from the list of specified regions, specified as an array of up to 15 ccTLD ("top-level domain") two-character values.
    /// If omitted, no restrictions are applied to the response.
    /// For example, to limit the regions to Germany and France:
    /// "includedRegionCodes": ["de", "fr"]
    /// If you specify both locationRestriction and includedRegionCodes, the results are located in the area of intersection of the two settings.
    /// </summary>
    [J("includedRegionCodes")][I(Condition = C.WhenWritingNull)] public string[]? IncludedRegionCodes { get; set; }

    /// <summary>
    /// The preferred language in which to return results.
    /// The results might be in mixed languages if the language used in input is different from the value specified by languageCode, or if the returned place does not have a translation from the local language to languageCode.
    /// You must use IETF BCP-47 language codes to specify the preferred language.
    /// If languageCode is not supplied, the API uses the value specified in the Accept-Language header.
    /// If neither is specified, the default is en.
    /// If you specify an invalid language code, the API returns an INVALID_ARGUMENT error.
    /// The preferred language has a small influence on the set of results that the API chooses to return, and the order in which they are returned.
    /// This also affects the API's ability to correct spelling errors.
    /// The API attempts to provide a street address that is readable for both the user and local population, while at the same time reflecting the user input.
    /// Place predictions are formatted differently depending on the user input in each request.
    /// Matching terms in the input parameter are chosen first, using names aligned with the language preference indicated by the languageCode parameter when available, while otherwise using names that best match the user input.
    /// Street addresses are formatted in the local language, in a script readable by the user when possible, only after matching terms have been picked to match the terms in the input parameter.
    /// All other addresses are returned in the preferred language, after matching terms have been chosen to match the terms in the input parameter.
    /// If a name is not available in the preferred language, the API uses the closest match.
    /// </summary>
    [J("languageCode")][I(Condition = C.WhenWritingNull)] public string? LanguageCode { get; set; }

    /// <summary>
    /// The region code used to format the response, specified as a ccTLD ("top-level domain") two-character value.
    /// Most ccTLD codes are identical to ISO 3166-1 codes, with some notable exceptions.
    /// For example, the United Kingdom's ccTLD is "uk" (.co.uk) while its ISO 3166-1 code is "gb" (technically for the entity of "The United Kingdom of Great Britain and Northern Ireland").
    /// If you specify an invalid region code, the API returns an INVALID_ARGUMENT error.
    /// The parameter can affect results based on applicable law.
    /// </summary>
    [J("regionCode")][I(Condition = C.WhenWritingNull)] public string? RegionCode { get; set; }

    /// <summary>
    /// The origin point from which to calculate straight-line distance to the destination (returned as distanceMeters).
    /// If this value is omitted, straight-line distance will not be returned.
    /// Must be specified as latitude and longitude coordinates:
    /// </summary>
    [J("origin")][I(Condition = C.WhenWritingNull)] public Shared.LatitudeLongitude? Origin { get; set; }

    //[J("inputOffset")] public int InputOffset { get; set; }

    /// <summary>
    /// If true, the response includes both place and query predictions.
    /// The default value is false, meaning the response only includes place predictions.
    /// </summary>
    [J("includeQueryPredictions")] public bool IncludeQueryPredictions { get; set; }

    /// <summary>
    /// Session tokens are user-generated strings that track Autocomplete (New) calls as "sessions."
    /// Autocomplete (New) uses session tokens to group the query and selection phases of a user autocomplete search into a discrete session for billing purposes.
    /// For more information, see Session tokens.
    /// </summary>
    [J("sessionToken")][I(Condition = C.WhenWritingNull)] public string? SessionToken { get; set; }
}
