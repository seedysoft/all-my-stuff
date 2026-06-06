using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Geography.Services.Routers;

public class CartoCiudadRouter(Settings.Api api, Microsoft.Extensions.Logging.ILogger logger) : RouterBase(api)
{
    public override async Task<List<Models.RouteModel>> GetRoutesAsync(ViewModels.TravelQueryModel model, CancellationToken cancellationToken)
    {
        // orig={origLat,origLng}&dest={destLat,destLng}
        RestRequest restRequest = new(string.Format(Api.UrlFormat, $"{model.Origin.Latitude},{model.Origin.Longitude}", $"{model.Destination.Latitude},{model.Destination.Longitude}"));
        RestResponse restResponse = await RestClient.ExecuteGetAsync(restRequest, cancellationToken);

        CartoCiudadResponse.Candidate[]? body = null;
        if (restResponse.IsSuccessStatusCode)
            body = restResponse.Content!.FromJson<CartoCiudadResponse.Candidate[]>();
        else
            _ = logger.LogAndHandle(restResponse.ErrorException, restResponse.Content ?? "ERROR", []);

        if (body == null)
            return [];

        //IEnumerable<Models.Shared.LatLngLiteral> CoordinatesQuery =
        //    from r in body.Routes
        //    from l in r.Legs
        //    from g in l.Polyline?.GeoJsonLinestring?.Coordinates ?? []
        //    select new Models.Shared.LatLngLiteral(g[1], g[0]);

        //return CoordinatesQuery.Distinct().ToList();

        return [];
    }
}

public class CartoCiudadResponse
{
    public Candidate[] Body { get; set; }

    public class Candidate
    {
        public string id { get; set; }
        public string province { get; set; }
        public string provinceCode { get; set; }
        public string comunidadAutonoma { get; set; }
        public string comunidadAutonomaCode { get; set; }
        public string muni { get; set; }
        public string muniCode { get; set; }
        public string type { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string poblacion { get; set; }
        public object geom { get; set; }
        public string tip_via { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
        public int portalNumber { get; set; }
        public bool noNumber { get; set; }
        public string stateMsg { get; set; }
        public object extension { get; set; }
        public int state { get; set; }
        public string countryCode { get; set; }
        public string refCatastral { get; set; }
    }
}
