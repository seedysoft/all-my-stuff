namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class TextAndMatches
{
    [J("text")]
    public string? Text { get; init; }

#if DEBUG

    [J("matches")]
    public Match[]? Matches { get; init; }

#endif
}
