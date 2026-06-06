namespace Seedysoft.Libs.Geography.Settings;

public readonly record struct RouteSettings
{
    /// <summary>
    /// CartoCiudad | OSRM
    /// </summary>
    public required string RouteImplementation { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required RouteApi[] RoutesApi { get; init; }
}
