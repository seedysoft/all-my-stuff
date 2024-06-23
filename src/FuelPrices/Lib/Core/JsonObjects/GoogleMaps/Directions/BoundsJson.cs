using System.Text.Json.Serialization;

namespace Seedysoft.FuelPrices.Lib.Core.JsonObjects.GoogleMaps.Directions;

public record BoundsJson
{
    [JsonPropertyName("northeast")]
    public LocationJson Northeast { get; set; } = default!;

    [JsonPropertyName("southwest")]
    public LocationJson Southwest { get; set; } = default!;
}
