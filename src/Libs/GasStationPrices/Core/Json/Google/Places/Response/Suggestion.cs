namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Response;

public record class Suggestion
{
    [J("placePrediction")] public required PlacePrediction PlacePrediction { get; set; }
}
