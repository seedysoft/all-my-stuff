using RestSharp;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.GasStationPrices.Settings;
using Seedysoft.Libs.GasStationPrices.ViewModels;

namespace Seedysoft.Libs.GasStationPrices.Services.Routers;

internal class OsrmRouter(Api api, Microsoft.Extensions.Logging.ILogger logger) : RouterBase(api)
{
    /// <summary>
    /// Obtiene las rutas entre el origen y el destino especificados en el modelo de consulta.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal override async Task<IList<(string NombreRuta, double[][] Coordenadas)>> GetRoutesAsync(TravelQueryModel model, CancellationToken cancellationToken)
    {
        // {origLng,origLat};{destLng,destLat}
        RestRequest restRequest = new(string.Format(Api.UrlFormat,
            $"{model.Orig.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{model.Orig.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}",
            $"{model.Dest.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{model.Dest.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}"));
        RestResponse restResponse = await RestClient.ExecuteGetAsync(restRequest, cancellationToken);

        OsrmResponse? body = null;
        if (restResponse.IsSuccessStatusCode)
            body = restResponse.Content!.FromJson<OsrmResponse>();
        else
            _ = logger.LogAndHandle(restResponse.ErrorException, restResponse.Content ?? "ERROR", []);

        if (body == null)
            return [];

        if (body.code != "Ok")
        {
            _ = logger.LogAndHandle(null, $"OSRM API returned code: {body.code}", []);
            return [];
        }

        // Coordinates are in the format: [lng, lat]
        // We need to invert them to [lat, lng] for our application
        IEnumerable<(string NombreRuta, double[][] Coordenadas)> Result =
            from r in body.trips
            select (r.weight_name, InvertLongitudeLatitude(r.geometry.coordinates));

        return [.. Result];
    }
}

public class OsrmResponse
{
    public string code { get; set; }
    public Trip[] trips { get; set; }
    public Waypoint[] waypoints { get; set; }
}

public class Trip
{
    public Leg[] legs { get; set; }
    public string weight_name { get; set; }
    public Geometry geometry { get; set; }
    public float weight { get; set; }
    public float duration { get; set; }
    public float distance { get; set; }
}

public class Geometry
{
    // Coordinates are in the format: [lng, lat]
    public double[][] coordinates { get; set; }
    public string type { get; set; }
}

public class Leg
{
    public object[] steps { get; set; }
    public float weight { get; set; }
    public string summary { get; set; }
    public float duration { get; set; }
    public float distance { get; set; }
}

public class Waypoint
{
    public int waypoint_index { get; set; }
    public float distance { get; set; }
    public string name { get; set; }
    public float[] location { get; set; }
    public string hint { get; set; }
    public int trips_index { get; set; }
}
