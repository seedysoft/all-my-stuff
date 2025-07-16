namespace Seedysoft.Libs.GoogleApis.Settings;

public record GoogleApisSettings
{
    private string apiKey = default!;
    public required string ApiKey
    {
        get => apiKey;
        init => apiKey = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }

    public required string FieldMask { get; init; }

    //public required DirectionsApi DirectionsApi { get; init; }

    //public required MapsApi MapsApi { get; init; }

    public required PlacesApi PlacesApi { get; init; }

    //public required RoutesApi RoutesApi { get; init; }
}
