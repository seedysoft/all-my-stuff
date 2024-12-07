namespace Seedysoft.Libs.GoogleApis.Settings;

public record class GoogleApisSettings
{
    private string apiKey = default!;
    public required string ApiKey
    {
        get => apiKey;
        init => apiKey = Cryptography.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }

    public required MapsApi MapsApi { get; init; }

    public required PlacesApi PlacesApi { get; init; }

    public required RoutesApi RoutesApi { get; init; }
}
