namespace Seedysoft.Libs.GoogleApis.Settings;

public record PlacesApi
{
    public required string UrlFormat { get; init; }

    //public static string GetUrl(string urlFormat, Json.Google.Response.Route route, IEnumerable<double[]> wayPoints)
    //{
    //    string queryParameters =
    //        $"origin={route.Legs?.FirstOrDefault()?.StartLocation?.LatLng.ToQueryString()}&" +
    //        $"destination={route.Legs?.FirstOrDefault()?.EndLocation?.LatLng.ToQueryString()}&" +
    //        $"waypoints={string.Join("|", wayPoints.Select(x => $"{x[0]}+{x[1]}"))}";

    //    return string.Format(urlFormat, queryParameters);
    //}
}
