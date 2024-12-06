namespace Seedysoft.Libs.GoogleApis.Json.Places.Response;

public record class Match
{
    [J("endOffset")] public long EndOffset { get; set; }
    [J("startOffset"), I(Condition = C.WhenWritingNull)] public long? StartOffset { get; set; }
}
