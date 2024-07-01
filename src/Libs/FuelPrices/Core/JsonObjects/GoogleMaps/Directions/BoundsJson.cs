namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.GoogleMaps.Directions;

public record BoundsJson
{
    [System.Text.Json.Serialization.JsonPropertyName("northeast")]
    public LocationJson Northeast { get; set; } = default!;

    [System.Text.Json.Serialization.JsonPropertyName("southwest")]
    public LocationJson Southwest { get; set; } = default!;
}
