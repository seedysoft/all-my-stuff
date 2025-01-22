namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class TextAndMatches
{
    [J("text")]
    public string? Text { get; init; }

    [J("matches")]
    public Match[]? Matches { get; init; }
}
