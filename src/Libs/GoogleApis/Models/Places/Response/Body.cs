namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public class Body
{
    [J("suggestions")]
    public required Suggestion[] Suggestions { get; init; }
}
