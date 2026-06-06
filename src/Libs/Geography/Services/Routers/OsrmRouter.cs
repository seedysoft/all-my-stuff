using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Geography.Services.Routers;

public class OsrmRouter(Settings.Api api, Microsoft.Extensions.Logging.ILogger logger) : RouterBase(api)
{
    public override async Task<List<Models.RouteModel>> GetRoutesAsync(ViewModels.TravelQueryModel model, CancellationToken cancellationToken)
    {
        // {origLng,origLat};{destLng,destLat}
        RestRequest restRequest = new(string.Format(Api.UrlFormat,
            $"{model.Origin.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{model.Origin.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}",
            $"{model.Destination.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{model.Destination.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}"));
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

        // TODO                             Return each trip as a RouteModel, and not only the coordinates. Also, consider returning all the trips, and not only the first one.
        //IEnumerable<Models.Shared.LatLngLiteral> CoordinatesQuery =
        //    from r in body.trips
        //    from g in r.geometry.coordinates
        //    select new Models.Shared.LatLngLiteral(g[1], g[0]);

        //return CoordinatesQuery.Distinct().ToList();

        return [];
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
    public float[][] coordinates { get; set; }
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
