namespace GoogleMapsLibrary.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IBlazorGoogleMapsKeyService
{
    public Task<Maps.MapApiLoadOptions> GetApiOptions();

    public bool IsApiInitialized { get; set; }
}
