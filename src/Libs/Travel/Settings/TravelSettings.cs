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

public class GeocodingApi : Api
{
    public required string Name { get; init; }

    public virtual string GetUrl(string text) => string.Format(UrlFormat, text);
}

public class RoutingApi : Api
{
    public required string Name { get; init; }
}

public abstract class Api
{
    /// <summary>
    /// Gets the URL format for the API endpoint.
    /// </summary>
    public required string UrlFormat { get; init; }
}
