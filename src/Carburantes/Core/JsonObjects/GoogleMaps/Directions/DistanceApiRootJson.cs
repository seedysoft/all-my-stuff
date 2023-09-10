using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.Core.JsonObjects.GoogleMaps.Directions;

public record DistanceApiRootJson
{
    [JsonPropertyName("geocoded_waypoints")]
    public GeocodedWaypointsJson[] GeocodedWaypoints { get; set; } = default!;

    [JsonPropertyName("routes")]
    public RouteJson[] Routes { get; set; } = default!;

    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;
}
