using System.Text.Json.Serialization;

namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.GoogleMaps.Directions;

public record GeocodedWaypointsJson
{
    [JsonPropertyName("geocoder_status")]
    public string GeocoderStatus { get; set; } = default!;

    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; } = default!;

    [JsonPropertyName("types")]
    public string[] Types { get; set; } = default!;
}
