namespace Seedysoft.Libs.Geography.Settings;

public abstract class Api
{
    /// <summary>
    /// Gets the URL format for the API endpoint.
    /// </summary>
    public required string UrlFormat { get; init; }
}
