//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using RestSharp;
//using Seedysoft.Libs.Core.Extensions;

//namespace Seedysoft.Libs.GoogleApis.Services;

//public class RoutesService(IConfiguration configuration, ILogger<RoutesService> logger) : GoogleApisService(configuration)
//{
//    public async Task<List<Models.Shared.LatLngLiteral>> GetRoutesAsync(
//        string origin,
//        string destination,
//        CancellationToken cancellationToken)
//    {
//        RestRequest restRequest = BuildRoutesRequest(origin, destination);
//        RestClient restClient = new(GoogleApisSettings.RoutesApi.UrlFormat);
//        RestResponse restResponse = await restClient.ExecutePostAsync(restRequest, cancellationToken);

//        Models.Routes.Response.Body? body = null;
//        if (restResponse.IsSuccessStatusCode)
//            body = restResponse.Content!.FromJson<Models.Routes.Response.Body>();
//        else
//            _ = logger.LogAndHandle(restResponse.ErrorException, restResponse.Content ?? "ERROR", []);

//        if (body == null)
//            return [];

//        IEnumerable<Models.Shared.LatLngLiteral> CoordinatesQuery =
//            from r in body.Routes
//            from l in r.Legs
//            from g in l.Polyline?.GeoJsonLinestring?.Coordinates ?? []
//            select new Models.Shared.LatLngLiteral(g[1], g[0]);

//        return CoordinatesQuery.Distinct().ToList();

//        RestRequest BuildRoutesRequest(string origin, string destination)
//        {
//            RestRequest restRequest = new();
//            restRequest = restRequest.AddHeader("X-Goog-Api-Key", GoogleApisSettings.ApiKey);
//            restRequest = restRequest.AddHeader("X-Goog-FieldMask", "routes.legs.polyline");

//            Models.Routes.Request.Body RoutesRequestBody = new()
//            {
//                Origin = new Models.Routes.Request.Waypoint() { Address = origin, },
//                Destination = new Models.Routes.Request.Waypoint() { Address = destination },
//                ComputeAlternativeRoutes = true,
//                PolylineEncoding = Models.Routes.Request.PolylineEncoding.GeoJsonLinestring,
//                RouteModifiers = new Models.Routes.Request.RouteModifiers()
//                {
//                    AvoidTolls = false,
//                    AvoidFerries = false,
//                    AvoidHighways = false,
//                    VehicleInfo = new Models.Routes.Request.VehicleInfo() { EmissionType = Models.Routes.Request.VehicleEmissionType.Gasoline, },
//                },
//                RoutingPreference = Models.Routes.Request.RoutingPreference.TrafficAware,
//                TravelMode = Models.Routes.Shared.RouteTravelMode.Drive,
//            };

//            return restRequest.AddJsonBody(RoutesRequestBody.ToJson(), ContentType.Json);
//        }
//    }
//}
