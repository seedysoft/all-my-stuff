namespace Seedysoft.Libs.GasStationPrices.Core.Settings;

public readonly record struct PlacesApi
{
    public required string UriFormat { get; init; }

    //public static string GetUri(string uriFormat, Json.Google.Response.Route route, IEnumerable<double[]> wayPoints)
    //{
    //    string queryParameters =
    //        $"origin={route.Legs?.FirstOrDefault()?.StartLocation?.LatLng.ToQueryString()}&" +
    //        $"destination={route.Legs?.FirstOrDefault()?.EndLocation?.LatLng.ToQueryString()}&" +
    //        $"waypoints={string.Join("|", wayPoints.Select(x => $"{x[0]}+{x[1]}"))}";

    //    return string.Format(uriFormat, queryParameters);
    //}
}
