namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class Suggestion
{
    [J("placePrediction")]
    public required PlacePrediction PlacePrediction { get; init; }
}
