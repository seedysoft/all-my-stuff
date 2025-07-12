namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class Match
{
    [J("endOffset")]
    public long EndOffset { get; init; }

    [J("startOffset"), I(Condition = C.WhenWritingNull)]
    public long? StartOffset { get; init; }
}
