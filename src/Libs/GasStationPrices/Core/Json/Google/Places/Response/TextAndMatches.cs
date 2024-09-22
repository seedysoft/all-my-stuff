namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Response;

public record class TextAndMatches
{
    [J("text")] public string? Text { get; set; }

    [J("matches")] public Match[]? Matches { get; set; }
}
