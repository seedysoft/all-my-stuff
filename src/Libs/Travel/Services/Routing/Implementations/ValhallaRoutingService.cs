using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Routing.Implementations;

internal partial class ValhallaRoutingService(ValhallaRoutingApi api, ILogger logger) : RoutingImplementationBase(api)
{
    internal override async Task<IReadOnlyList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(
        Models.Location orig
        , Models.Location dest
        , CancellationToken cancellationToken)
    {
        RequestObject requestObject = new(orig, dest);

        RestRequest restRequest = new(api.GetUrl(requestObject));

        RestResponse restResponse = await RestClient.GetAsync(restRequest, cancellationToken);

        ResponseObject? body = null;
        if (restResponse.IsSuccessStatusCode)
            body = restResponse.Content!.FromJson<ResponseObject>();
        else
            _ = logger.LogAndHandle(restResponse.ErrorException, restResponse.Content ?? "ERROR", []);

        if (body == null)
            return [];

        if (!string.Equals("Ok", body.Code, StringComparison.InvariantCultureIgnoreCase))
        {
            _ = logger.LogAndHandle(null, $"{api.Name} returned code: {body.Code}", []);
            return [];
        }

        // Coordinates are in the format: [lng, lat]
        // We need to invert them to [lat, lng] for our application
        return [.. 
            (IEnumerable<(string NombreRuta, double[,] Coordenadas)>)(body.Routes?.Select((r, i) =>
            (r.Legs.First().Summary ?? i.ToString(),
            InvertLongitudeLatitude(Extensions.ArrayExtensions.To2D(r.Geometry?.Coordinates ?? [])))) ?? [])];
    }

    internal class RequestObject
    {
        public RequestObject(Models.Location orig, Models.Location dest)
        {
            Locations[0] = new Location(orig.Lat, orig.Lon);
            Locations[1] = new Location(dest.Lat, dest.Lon);
        }

        [J("alternates")] public int Alternates { get; } = 3;
        [J("banner_instructions")] public bool Banner_instructions { get; } = true;
        [J("costing")] public string Costing { get; } = "auto";
        [J("directions_options")] public Directions_Options Directions_options { get; } = new Directions_Options();
        [J("directions_type")] public string Directions_type { get; } = "none"; // none | maneuvers | instructions
        [J("format")] public string Format { get; } = "osrm";
        [J("locations")] public Location[] Locations { get; } = new Location[2];
        [J("roundabout_exits")] public bool Roundabout_exits { get; } = false;
        [J("shape_format")] public string Shape_format { get; } = "geojson";
    }
    internal class Directions_Options
    {
        [J("units")] public string Units { get; } = "kilometers";
    }
    internal class Location(decimal lat, decimal lon)
    {
        [J("lat")] public double Lat { get; set; } = (double)lat;
        [J("lon")] public double Lon { get; set; } = (double)lon;
    }

    internal class ResponseObject
    {
        [J("routes")] public Route[]? Routes { get; init; }
        //[J("waypoints")] public Waypoint[] waypoints { get; init; }
        [J("code")] public required string Code { get; init; }
    }
    internal class Route
    {
        [J("weight_name")] public required string Weight_name { get; init; }
        [J("weight")] public float Weight { get; init; }
        [J("duration")] public float Duration { get; init; }
        [J("distance")] public float Distance { get; init; }
        [J("legs")] public required Leg[] Legs { get; init; }
        [J("geometry")] public Geometry? Geometry { get; init; }
    }
    internal class Geometry
    {
        [J("coordinates")] public double[][]? Coordinates { get; init; }
        [J("type")] public string? Type { get; init; }
    }
    internal class Leg
    {
        //public object[] via_waypoints { get; init; }
        //public Admin[] admins { get; init; }
        [J("weight")] public float Weight { get; init; }
        [J("duration")] public float Duration { get; init; }
        //[J("steps")] public object[] steps { get; init; }
        [J("distance")] public float Distance { get; init; }
        [J("summary")] public string Summary { get; init; } = default!;
    }
    //internal class Admin
    //{
    //    [J("iso_3166_1_alpha3")]  public string iso_3166_1_alpha3 { get; init; }
    //    [J("iso_3166_1")] public string iso_3166_1 { get; init; }
    //}
    //internal class Waypoint
    //{
    //    [J("waypoint_index")] public int waypoint_index { get; init; }
    //    [J("trips_index")] public int trips_index { get; init; }
    //    [J("trips_index")] public float trips_index { get; init; }
    //    [J("name")] public string name { get; init; }
    //    [J("location")] public float[] location { get; init; }
    //}
}

internal record ValhallaRoutingApi : Settings.RoutingApi
{
    internal ValhallaRoutingApi(Settings.RoutingApi original) : base(original.Name, original.UrlFormat) { }

    public override string GetUrl<T>(T obj) => string.Format(UrlFormat, obj.ToJson(allowReadOnlyProperties: true));
}
