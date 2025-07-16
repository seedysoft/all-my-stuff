namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class StructuredFormat
{
    [J("mainText")]
    public TextAndMatches? MainText { get; init; }

    [J("secondaryText")]
    public TextAndMatches? SecondaryText { get; init; }
}
