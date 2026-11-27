using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Constants;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services;

internal class OsrmService(HttpClient httpClient, ILogger logger) :
    Interfaces.IRoutingService
{

    public static IHttpClientBuilder ConfigureHttpClient(
        Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        return hostApplicationBuilder.Services.AddHttpClient<OsrmService>()

            .ConfigureHttpClient(static configureClient =>
            {
                configureClient.BaseAddress = new Uri("http://router.project-osrm.org/");

                configureClient.DefaultRequestHeaders.Accept.Clear();
                configureClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                configureClient.DefaultRequestHeaders.AcceptEncoding.Clear();
                configureClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                configureClient.DefaultRequestHeaders.Connection.Clear();
                configureClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
                //configureClient.DefaultRequestHeaders.Host = System.Net.Dns.GetHostName(); // Do not use
                configureClient.DefaultRequestHeaders.UserAgent.Clear();
                configureClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{nameof(OsrmService)}/1.0 (Windows 10; Contact: seedysoft@gmail.com)");
            })

            .ConfigurePrimaryHttpMessageHandler(static () =>
            {
                return new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.All,
                    AllowAutoRedirect = true,
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                    PreAuthenticate = true,
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    SslProtocols = System.Security.Authentication.SslProtocols.None,
                };
            });
    }
    public async Task<IReadOnlyList<(string NombreRuta, double[,] Coordenadas)>> RouteAsync(
        Core.Models.Location orig,
        Core.Models.Location dest,
        CancellationToken cancellationToken)
    {
        // {origLng,origLat};{destLng,destLat}
        HttpRequestMessage httpRequestMessage = new()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(string.Format(
                Globalization.NumberFormatInfoInvariant,
                "trip/v1/driving/{0},{1};{2},{3}",
                orig.Longitude,
                orig.Latitude,
                dest.Longitude,
                dest.Latitude)),
        };
        //roundtrip=false&source=first&destination=last&geometries=geojson
        _ = httpRequestMessage.Options.TryAdd("roundtrip", false);
        _ = httpRequestMessage.Options.TryAdd("source", "first");
        _ = httpRequestMessage.Options.TryAdd("destination", "last");
        _ = httpRequestMessage.Options.TryAdd("geometries", "geojson");

        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            OsrmResponse? body = await httpResponseMessage.Content.FromJsonAsync<OsrmResponse>(cancellationToken);

            if (body != null)
            {
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
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(nameof(OsrmService) + "." + nameof(RouteAsync) + ": {StatusCode} {ReasonPhrase}", httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
        }

        return [];

        static double[,] InvertLongitudeLatitude(double[,] matrix)
        {
            double[,] newArray = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for (int row = 0; row < newArray.GetLength(0); row++)
            {
                for (int col = 0; col < newArray.GetLength(1); col++)
                {
                    newArray.SetValue(matrix[row, col == 0 ? 1 : 0], [row, col]);
                }
            }

            return newArray;
        }
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
