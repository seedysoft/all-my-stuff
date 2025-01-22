namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class PlacePrediction
{
    [J("place")]
    public string? Place { get; init; }

    [J("placeId")]
    public string? PlaceId { get; init; }

    [J("text")]
    public TextAndMatches? Text { get; init; }

    [J("structuredFormat")]
    public StructuredFormat? StructuredFormat { get; init; }

    [J("types")]
    public string[]? Types { get; init; }

    public override string ToString() => Text?.Text ?? "Unkown";
}
