using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.Core.JsonObjects.GoogleMaps.Directions;

public record RouteJson
{
    [JsonPropertyName("bounds")]
    public BoundsJson Bounds { get; set; } = default!;

    [JsonPropertyName("copyrights")]
    public string Copyrights { get; set; } = default!;

    /// <summary>
    /// A route with no waypoints will contain exactly one leg within the legs array
    /// </summary>
    [JsonPropertyName("legs")]
    public LegJson[] Legs { get; set; } = default!;

    [JsonPropertyName("overview_polyline")]
    public PolylineJson OverviewPolyline { get; set; } = default!;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = default!;

    [JsonPropertyName("warnings")]
    public object[] Warnings { get; set; } = default!;

    [JsonPropertyName("waypoint_order")]
    public object[] WaypointOrder { get; set; } = default!;
}
