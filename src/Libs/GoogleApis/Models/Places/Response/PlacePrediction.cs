namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class PlacePrediction
{
#if DEBUG

    [J("place")]
    public string? Place { get; init; }

    [J("placeId")]
    public string? PlaceId { get; init; }

#endif

    [J("text")]
    public TextAndMatches? Text { get; init; }

#if DEBUG

    [J("structuredFormat")]
    public StructuredFormat? StructuredFormat { get; init; }

    [J("types")]
    public string[]? Types { get; init; }

#endif

    public override string ToString() => Text?.Text ?? "Unkown";
}
