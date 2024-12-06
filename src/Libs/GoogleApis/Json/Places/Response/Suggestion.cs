namespace Seedysoft.Libs.GoogleApis.Json.Places.Response;

public record class Suggestion
{
    [J("placePrediction")] public required PlacePrediction PlacePrediction { get; set; }
}
