namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

public record Detail
{
    [J("@type")]
    public string? Type { get; init; }
}
