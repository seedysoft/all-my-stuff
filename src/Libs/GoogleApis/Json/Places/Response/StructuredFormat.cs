namespace Seedysoft.Libs.GoogleApis.Json.Places.Response;

public record class StructuredFormat
{
    [J("mainText")] public TextAndMatches? MainText { get; set; }
    [J("secondaryText")] public TextAndMatches? SecondaryText { get; set; }
}
