namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.GoogleMaps.Directions;

public record LegJson
{
    [System.Text.Json.Serialization.JsonPropertyName("distance")]
    public TextAndValueJson Distance { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("duration")]
    public TextAndValueJson Duration { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("end_address")]
    public string EndAddress { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("end_location")]
    public LocationJson EndLocation { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("start_address")]
    public string StartAddress { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("start_location")]
    public LocationJson StartLocation { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("steps")]
    public StepJson[] Steps { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("traffic_speed_entry")]
    public object[] TrafficSpeedEntry { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("via_waypoint")]
    public object[] ViaWaypoint { get; set; } = default!;
}
