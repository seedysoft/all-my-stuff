using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Routing;

internal class OpenSourceRoutingMachine(Settings.RoutingApi api, Microsoft.Extensions.Logging.ILogger logger) : RoutingBase(api)
{
    /// <summary>
    /// Obtiene las rutas entre el origen y el destino especificados en el modelo de consulta.
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="dest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal override async Task<IList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(
        Models.Location orig
        , Models.Location dest
        , CancellationToken cancellationToken)
    {
        // {origLng,origLat};{destLng,destLat}
        RestRequest restRequest = new(string.Format(RoutingApi.UrlFormat,
            $"{orig.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{orig.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}",
            $"{dest.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{dest.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}"));
        RestResponse restResponse = await RestClient.ExecuteGetAsync(restRequest, cancellationToken);

        OsrmResponse? body = null;
        if (restResponse.IsSuccessStatusCode)
            body = restResponse.Content!.FromJson<OsrmResponse>();
        else
            _ = logger.LogAndHandle(restResponse.ErrorException, restResponse.Content ?? "ERROR", []);

        if (body == null)
            return [];

        if ((body.Code ?? string.Empty) != "Ok")
        {
            _ = logger.LogAndHandle(null, $"OSRM API returned code: {body.Code}", []);
            return [];
        }

        // Coordinates are in the format: [lng, lat]
        // We need to invert them to [lat, lng] for our application
        IEnumerable<(string NombreRuta, double[,] Coordenadas)> Result =
            from r in body.Trips
            select (r.WeightName, InvertLongitudeLatitude(Extensions.ArrayExtensions.To2D(r.Geometry?.Coordinates ?? [])));

        return [.. Result];
    }

    public class OsrmResponse
    {
        [J("code")] public string? Code { get; set; }
        [J("trips")] public Trip[]? Trips { get; set; }
        [J("waypoints")] public Waypoint[]? Waypoints { get; set; }
    }

    public class Trip
    {
        [J("legs")] public Leg[]? Legs { get; set; }
        [J("weight_name")] public string? WeightName { get; set; }
        [J("geometry")] public Geometry? Geometry { get; set; }
        [J("weight")] public float Weight { get; set; }
        [J("duration")] public float Duration { get; set; }
        [J("distance")] public float Distance { get; set; }
    }

    public class Geometry
    {
        // Coordinates are in the format: [lng, lat]
        [J("coordinates")] public double[][]? Coordinates { get; set; }
        [J("type")] public string? Type { get; set; }
    }

    public class Leg
    {
        [J("steps")] public object[]? Steps { get; set; }
        [J("weight")] public float Weight { get; set; }
        [J("summary")] public string? Summary { get; set; }
        [J("duration")] public float Duration { get; set; }
        [J("distance")] public float Distance { get; set; }
    }

    public class Waypoint
    {
        [J("waypoint_index")] public int WaypointIndex { get; set; }
        [J("distance")] public float Distance { get; set; }
        [J("name")] public string? Name { get; set; }
        [J("location")] public float[]? Location { get; set; }
        [J("hint")] public string? Hint { get; set; }
        [J("trips_index")] public int TripsIndex { get; set; }
    }
}
