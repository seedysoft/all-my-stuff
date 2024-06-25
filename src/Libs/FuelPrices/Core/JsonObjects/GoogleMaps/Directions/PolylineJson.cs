namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.GoogleMaps.Directions;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public record PolylineJson
{
    [System.Text.Json.Serialization.JsonPropertyName("points")]
    public string Points { get; set; } = default!;

    public LocationJson[] Locations()
    {
        return (LocationJson[])(string.IsNullOrEmpty(Points)
            ? Enumerable.Empty<LocationJson>()
            : Extensions.GoogleHelper.Decode(Points).ToArray());
    }

    private string GetDebuggerDisplay() => $"{Points}";
}
