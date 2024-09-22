namespace Seedysoft.Libs.GasStationPrices.Services;

public class BlazorGoogleMapsKeyService(Core.Settings.SettingsRoot settingsRoot) : GoogleMapsComponents.IBlazorGoogleMapsKeyService
{
    public bool IsApiInitialized { get; set; }

    public Task<GoogleMapsComponents.Maps.MapApiLoadOptions> GetApiOptions()
    {
        IsApiInitialized = true;

        return Task.FromResult(new GoogleMapsComponents.Maps.MapApiLoadOptions(settingsRoot.GoogleMapsPlatform.ApiKey) { Version = "beta", });
    }
}
