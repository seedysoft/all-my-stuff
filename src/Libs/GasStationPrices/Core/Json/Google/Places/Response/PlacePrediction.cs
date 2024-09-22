namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Response;

public record class PlacePrediction
{
    [J("place")] public string? Place { get; set; }
    [J("placeId")] public string? PlaceId { get; set; }
    [J("text")] public TextAndMatches? Text { get; set; }
    [J("structuredFormat")] public StructuredFormat? StructuredFormat { get; set; }
    [J("types")] public string[]? Types { get; set; }

    public override string ToString() => Text?.Text ?? "Unkown";
}
