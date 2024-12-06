namespace Seedysoft.Libs.GoogleApis.Json.Places.Response;

public record class Body
{
    [J("suggestions")] public required Suggestion[] Suggestions { get; set; }
}
