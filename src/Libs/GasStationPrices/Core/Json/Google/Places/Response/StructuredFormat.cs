namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Response;

public record class StructuredFormat
{
    [J("mainText")] public TextAndMatches? MainText { get; set; }
    [J("secondaryText")] public TextAndMatches? SecondaryText { get; set; }
}
