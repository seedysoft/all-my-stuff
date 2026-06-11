namespace Seedysoft.Libs.GasStationPrices.Settings;

public readonly record struct GeographySettings
{
    /// <summary>
    /// 
    /// </summary>
    public required PlacesApi PlacesApi { get; init; }

    public required RouteSettings RouteSettings { get; init; }
}
