namespace Seedysoft.Libs.Geography.Settings;

public readonly record struct PlacesApi
{
    /// <summary>
    /// 
    /// </summary>
    public required string UrlFormat { get; init; }

    public string GetUrl(string textToFind) => string.Format(UrlFormat, textToFind);
}
