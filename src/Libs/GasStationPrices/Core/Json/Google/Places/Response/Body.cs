namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Response;

public record class Body
{
    [J("suggestions")] public required Suggestion[] Suggestions { get; set; }
}
