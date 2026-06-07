namespace Seedysoft.Libs.Geography.Settings;

public readonly record struct RouteSettings
{
    /// <summary>
    /// CartoCiudad | MapboxDirections | OSRM
    /// </summary>
    public required string CurrentImplementation { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required RouteApi[] RouteApis { get; init; }
}
