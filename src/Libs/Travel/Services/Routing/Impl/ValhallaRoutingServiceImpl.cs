using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Routing.Impl;

internal partial class ValhallaRoutingServiceImpl(
    IHttpClientFactory httpClientFactory
    , ValhallaRoutingApi api
    , ILogger logger) : RoutingServiceImplBase(httpClientFactory, api)
{
    internal override async Task<IReadOnlyList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(
        Models.Location orig
        , Models.Location dest
        , CancellationToken cancellationToken)
    {
        RequestObject requestObject = new(orig, dest);
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(Api.GetUrl(requestObject), cancellationToken);

        ResponseObject? body = null;
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            body = await httpResponseMessage.Content.FromJsonAsync<ResponseObject>(cancellationToken);
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("ValhallaRoutingServiceImpl: FindPlacesAsync: {StatusCode} {ReasonPhrase}", httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
        }

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
                InvertLongitudeLatitude(Extensions.ArrayExtensions.To2D(r.Geometry?.Coordinates ?? [])))) ?? [])
            ];
    }

    internal class RequestObject
    {
        public RequestObject(Models.Location orig, Models.Location dest)
        {
            Locations[0] = new Location(orig.Latitude, orig.Longitude);
            Locations[1] = new Location(dest.Latitude, dest.Longitude);
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
    internal class Location(double lat, double lon)
    {
        [J("lat")] public double Lat { get; set; } = lat;
        [J("lon")] public double Lon { get; set; } = lon;
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

internal record ValhallaRoutingApi : Settings.RoutingServiceApi
{
    internal ValhallaRoutingApi(Settings.RoutingServiceApi original) : base(original.Name, original.UrlFormat) { }

    public override string GetUrl<T>(T obj) => string.Format(UrlFormat, obj.ToJson(allowReadOnlyProperties: true));
}
