using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.CoreLib.JsonObjects.GoogleMaps.Directions;

public record StepJson
{
    [JsonPropertyName("distance")]
    public TextAndValueJson Distance { get; set; } = default!;

    [JsonPropertyName("duration")]
    public TextAndValueJson Duration { get; set; } = default!;

    [JsonPropertyName("end_location")]
    public LocationJson EndLocation { get; set; } = default!;

    [JsonPropertyName("html_instructions")]
    public string HtmlInstructions { get; set; } = default!;

    [JsonPropertyName("polyline")]
    public PolylineJson Polyline { get; set; } = default!;

    [JsonPropertyName("start_location")]
    public LocationJson StartLocation { get; set; } = default!;

    [JsonPropertyName("travel_mode")]
    public string TravelMode { get; set; } = default!;

    [JsonPropertyName("maneuver")]
    public string Maneuver { get; set; } = default!;
}
