using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.CoreLib.JsonObjects.GoogleMaps.Directions;

public record LegJson
{
    [JsonPropertyName("distance")]
    public TextAndValueJson Distance { get; set; } = default!;

    [JsonPropertyName("duration")]
    public TextAndValueJson Duration { get; set; } = default!;

    [JsonPropertyName("end_address")]
    public string EndAddress { get; set; } = default!;

    [JsonPropertyName("end_location")]
    public LocationJson EndLocation { get; set; } = default!;

    [JsonPropertyName("start_address")]
    public string StartAddress { get; set; } = default!;

    [JsonPropertyName("start_location")]
    public LocationJson StartLocation { get; set; } = default!;

    [JsonPropertyName("steps")]
    public StepJson[] Steps { get; set; } = default!;

    [JsonPropertyName("traffic_speed_entry")]
    public object[] TrafficSpeedEntry { get; set; } = default!;

    [JsonPropertyName("via_waypoint")]
    public object[] ViaWaypoint { get; set; } = default!;
}
