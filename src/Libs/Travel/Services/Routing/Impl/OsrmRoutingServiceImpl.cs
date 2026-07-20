using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Routing.Impl;

/// <summary>
/// Implementation of the routing service that integrates with the Open Source Routing Machine (OSRM) API.
/// This class handles communication with OSRM to retrieve routing information between two geographic locations.
/// </summary>
internal class OsrmRoutingServiceImpl(Settings.RoutingServiceApi api, Microsoft.Extensions.Logging.ILogger logger) : RoutingServiceImplBase(api)
{
    /// <summary>
    /// Obtains the routes between the specified origin and destination locations.
    /// Communicates with the OSRM API, parses the response, and transforms coordinate format from [longitude, latitude] to [latitude, longitude].
    /// </summary>
    /// <param name="orig">The origin location coordinates.</param>
    /// <param name="dest">The destination location coordinates.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A read-only list of tuples containing route names and their corresponding coordinates in [latitude, longitude] format.
    /// Returns an empty list if the API call fails or returns an error.
    /// </returns>
    internal override async Task<IReadOnlyList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(
        Models.Location orig
        , Models.Location dest
        , CancellationToken cancellationToken)
    {
        // {origLng,origLat};{destLng,destLat}
        RestRequest restRequest = new(string.Format(RoutingApi.UrlFormat,
            $"{orig.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{orig.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}",
            $"{dest.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)},{dest.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}"));
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
        return body.Trips?
            .Where(x => x.WeightName != null && x.Geometry?.Coordinates != null)
            .Select(x => (
                NombreRuta: x.WeightName!,
                Coordenadas: InvertLongitudeLatitude(Extensions.ArrayExtensions.To2D(x.Geometry!.Coordinates))!
            ))
            // Non-null after Where
            .ToList() ?? [];
    }

    /// <summary>
    /// Represents the root response structure from the OSRM API.
    /// </summary>
    internal class OsrmResponse
    {
        /// <summary>
        /// The status code returned by the OSRM API. Expected value is "Ok" for successful requests.
        /// </summary>
        [J("code")] public string? Code { get; set; }

        /// <summary>
        /// An array of trip objects representing routes from origin to destination.
        /// </summary>
        [J("trips")] public Trip[]? Trips { get; set; }

        /// <summary>
        /// An array of waypoint objects corresponding to the input coordinates.
        /// </summary>
        [J("waypoints")] public Waypoint[]? Waypoints { get; set; }
    }

    /// <summary>
    /// Represents a single trip/route segment in the OSRM response.
    /// Contains geometric and metric information about the route.
    /// </summary>
    internal class Trip
    {
        /// <summary>
        /// An array of legs that compose this trip. Each leg represents a segment of the journey.
        /// </summary>
        [J("legs")] public Leg[]? Legs { get; set; }

        /// <summary>
        /// The name or identifier of the weight metric used for this trip (e.g., "duration", "distance").
        /// </summary>
        [J("weight_name")] public string? WeightName { get; set; }

        /// <summary>
        /// The geometry object containing the coordinate path of this trip.
        /// </summary>
        [J("geometry")] public Geometry? Geometry { get; set; }

        /// <summary>
        /// The total weight value of this trip based on the weight metric.
        /// </summary>
        [J("weight")] public float Weight { get; set; }

        /// <summary>
        /// The total duration of this trip in seconds.
        /// </summary>
        [J("duration")] public float Duration { get; set; }

        /// <summary>
        /// The total distance of this trip in meters.
        /// </summary>
        [J("distance")] public float Distance { get; set; }
    }

    /// <summary>
    /// Represents the geometric path of a route with coordinates in [longitude, latitude] format.
    /// Note: Coordinates are in GeoJSON format [lng, lat] and require inversion for application use.
    /// </summary>
    internal class Geometry
    {
        /// <summary>
        /// A two-dimensional array of coordinates representing the route path.
        /// Each coordinate is in the format [longitude, latitude].
        /// </summary>
        [J("coordinates")] public double[][]? Coordinates { get; set; }

        /// <summary>
        /// The type of geometry object (typically "LineString" for routes).
        /// </summary>
        [J("type")] public string? Type { get; set; }
    }

    /// <summary>
    /// Represents a single leg (segment) of a trip between two waypoints.
    /// Contains detailed information about this segment of the journey.
    /// </summary>
    internal class Leg
    {
        /// <summary>
        /// An array of detailed turn-by-turn navigation steps for this leg.
        /// </summary>
        [J("steps")] public object[]? Steps { get; set; }

        /// <summary>
        /// The total weight value for this leg based on the weight metric.
        /// </summary>
        [J("weight")] public float Weight { get; set; }

        /// <summary>
        /// A text summary describing this leg of the journey.
        /// </summary>
        [J("summary")] public string? Summary { get; set; }

        /// <summary>
        /// The duration of this leg in seconds.
        /// </summary>
        [J("duration")] public float Duration { get; set; }

        /// <summary>
        /// The distance of this leg in meters.
        /// </summary>
        [J("distance")] public float Distance { get; set; }
    }

    /// <summary>
    /// Represents a waypoint from the OSRM response, providing information about input coordinate snapping.
    /// </summary>
    internal class Waypoint
    {
        /// <summary>
        /// The index of this waypoint in the list of input waypoints.
        /// </summary>
        [J("waypoint_index")] public int WaypointIndex { get; set; }

        /// <summary>
        /// The distance from the input coordinate to the snapped point on the road network in meters.
        /// </summary>
        [J("distance")] public float Distance { get; set; }

        /// <summary>
        /// The name of the road or location where the waypoint was snapped.
        /// </summary>
        [J("name")] public string? Name { get; set; }

        /// <summary>
        /// The geographic location coordinates of the snapped waypoint in [longitude, latitude] format.
        /// </summary>
        [J("location")] public float[]? Location { get; set; }

        /// <summary>
        /// An opaque hint that can be used in subsequent requests to speed up processing.
        /// </summary>
        [J("hint")] public string? Hint { get; set; }

        /// <summary>
        /// The index of the trip this waypoint belongs to in the trips array.
        /// </summary>
        [J("trips_index")] public int TripsIndex { get; set; }
    }
}
