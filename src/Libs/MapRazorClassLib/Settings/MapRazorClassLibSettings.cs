namespace Seedysoft.Libs.MapRazorClassLib.Settings;

public record MapRazorClassLibSettings
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
