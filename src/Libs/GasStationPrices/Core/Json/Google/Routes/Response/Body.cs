namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

public class Body
{
    /// <summary>
    /// Contains an array of computed routes (up to three) when you specify compute_alternatives_routes, and contains just one route when you don't. When this array contains multiple entries, the first one is the most recommended route. If the array is empty, then it means no route could be found.
    /// </summary>
    [J("routes")] public required Route[] Routes { get; set; }
    /// <summary>
    /// In some cases when the server is not able to compute the route results with all of the input preferences, it may fallback to using a different way of computation. When fallback mode is used, this field contains detailed info about the fallback response. Otherwise this field is unset.
    /// </summary>
    [J("fallbackInfo"), I(Condition = C.WhenWritingNull)] public FallbackInfo? FallbackInfo { get; set; }
    /// <summary>
    /// Contains geocoding response info for waypoints specified as addresses.
    /// </summary>
    [J("geocodingResults"), I(Condition = C.WhenWritingNull)] public GeocodingResults? GeocodingResults { get; set; }
}
