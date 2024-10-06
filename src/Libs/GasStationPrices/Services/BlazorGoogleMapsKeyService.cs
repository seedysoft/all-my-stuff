namespace Seedysoft.Libs.GasStationPrices.Services;

public class BlazorGoogleMapsKeyService(Core.Settings.SettingsRoot settingsRoot) : GoogleMapsLibrary.Interfaces.IBlazorGoogleMapsKeyService
{
    public bool IsApiInitialized { get; set; }

    public Task<GoogleMapsLibrary.Maps.MapApiLoadOptions> GetApiOptions()
    {
        IsApiInitialized = true;

        return Task.FromResult(new GoogleMapsLibrary.Maps.MapApiLoadOptions(settingsRoot.GoogleMapsPlatform.ApiKey) { Version = "beta", });
    }
}
