namespace Seedysoft.Libs.Travel.Settings;

public readonly record struct TravelSettings
{
    /// <summary>
    /// 
    /// </summary>
    public required GeocodingSettings GeocodingSettings { get; init; }

    public required RoutingSettings RoutingSettings { get; init; }
}

public readonly record struct GeocodingSettings
{
    public required string CurrentImplementation { get; init; }

    public required GeocodingApi[] GeocodingApis { get; init; }
}

public readonly record struct RoutingSettings
{
    /// <summary>
    /// MapboxDirections | OSRM
    /// </summary>
    public required string CurrentImplementation { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required RoutingApi[] RoutingApis { get; init; }
}

public record GeocodingApi(string Name, string UrlFormat) : Api(UrlFormat);

public record RoutingApi(string Name, string UrlFormat) : Api(UrlFormat);

/// <param name="UrlFormat">Gets the URL format for the API endpoint.</param>
public abstract record Api(string UrlFormat)
{
    public virtual string GetUrl<T>(T text) => string.Format(UrlFormat, text);
}
