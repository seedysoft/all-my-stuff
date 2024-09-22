namespace Seedysoft.Libs.GasStationPrices.Core.Settings;

public record class GoogleMapsPlatform
{
    private string apiKey = default!;
    public required string ApiKey
    {
        get => apiKey;
        init => apiKey = Cryptography.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }

    public required Maps Maps { get; init; }

    public required PlacesApi PlacesApi { get; init; }

    public required RoutesApi RoutesApi { get; init; }
}
