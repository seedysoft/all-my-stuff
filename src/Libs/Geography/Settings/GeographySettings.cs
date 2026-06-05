namespace Seedysoft.Libs.Geography.Settings;

public readonly record struct GeographySettings
{
    /// <summary>
    /// 
    /// </summary>
    public required PlacesApi PlacesApi { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public required RoutesApi RoutesApi { get; init; }
}
