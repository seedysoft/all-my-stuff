namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.GoogleMaps.Directions;

public record StepJson
{
    [System.Text.Json.Serialization.JsonPropertyName("distance")]
    public TextAndValueJson Distance { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("duration")]
    public TextAndValueJson Duration { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("end_location")]
    public LocationJson EndLocation { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("html_instructions")]
    public string HtmlInstructions { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("polyline")]
    public PolylineJson Polyline { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("start_location")]
    public LocationJson StartLocation { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("travel_mode")]
    public string TravelMode { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("maneuver")]
    public string Maneuver { get; set; } = default!;
}
