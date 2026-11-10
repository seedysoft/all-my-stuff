namespace Seedysoft.Libs.Travel.Settings;

public readonly record struct TravelSettings
{
    public required GeocodingSettings GeocodingSettings { get; init; }

    public required ReverseSettings ReverseSettings { get; init; }

    public required RoutingSettings RoutingSettings { get; init; }
}

public readonly record struct GeocodingSettings
{
    public required string CurrentImplName { get; init; }

    public required GeocodingServiceApi[] GeocodingApis { get; init; }
}

public readonly record struct ReverseSettings
{
    public required string CurrentImplName { get; init; }

    public required ReverseServiceApi[] ReverseApis { get; init; }
}

public readonly record struct RoutingSettings
{
    /// <summary>
    /// MapboxDirections | OSRM
    /// </summary>
    public required string CurrentImplName { get; init; }

    public required RoutingServiceApi[] RoutingApis { get; init; }
}

public record GeocodingServiceApi(string Name, string UrlFormat) : Api(UrlFormat);

public record ReverseServiceApi(string Name, string UrlFormat) : Api(UrlFormat)
{
    public override string GetUrl<T>(T obj)
    {
        return obj is Core.Models.Location location
            ? string.Format(UrlFormat, location.Latitude, location.Longitude)
            : base.GetUrl<T>(obj);
    }
}

public record RoutingServiceApi(string Name, string UrlFormat) : Api(UrlFormat);

/// <param name="UrlFormat">Gets the URL format for the API endpoint.</param>
public abstract record Api(string UrlFormat)
{
    public virtual string GetUrl<T>(T obj) => string.Format(UrlFormat, obj);
}
