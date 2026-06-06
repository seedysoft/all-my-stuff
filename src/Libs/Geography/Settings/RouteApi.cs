namespace Seedysoft.Libs.Geography.Settings;

public class RouteApi : Api
{
    public required string RouteName { get; init; }

    ///// <summary>
    ///// http://www.cartociudad.es/services/api/route?orig=x,y&dest=x,y&locale=es&vehicle=[CAR|WALK]
    ///// </summary>
    ///// <remarks>
    ///// Hay que introducir la latitud y la longitud geográficas en grado con decimales de la siguiente forma latitud, longitud.
    ///// </remarks>
    //public required string UrlFormat { get; init; }

    //public static string GetUrl(string urlFormat, Json.Google.Response.Route route, IEnumerable<double[]> wayPoints)
    //{
    //    string queryParameters =
    //        $"origin={route.Legs?.FirstOrDefault()?.StartLocation?.LatLng.ToQueryString()}&" +
    //        $"destination={route.Legs?.FirstOrDefault()?.EndLocation?.LatLng.ToQueryString()}&" +
    //        $"waypoints={string.Join("|", wayPoints.Select(x => $"{x[0]}+{x[1]}"))}";

    //    return string.Format(urlFormat, queryParameters);
    //}
}
