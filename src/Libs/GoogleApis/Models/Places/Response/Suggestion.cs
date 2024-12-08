namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public record class Suggestion
{
    [J("placePrediction")]
    public required PlacePrediction PlacePrediction { get; init; }
}
