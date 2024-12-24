namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public record class Body
{
    [J("suggestions")]
    public required Suggestion[] Suggestions { get; init; }
}
