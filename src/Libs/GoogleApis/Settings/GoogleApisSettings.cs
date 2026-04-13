namespace Seedysoft.Libs.GoogleApis.Settings;

public record GoogleApisSettings
{
    public required string ApiKey
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    } = default!;

    public required string FieldMask { get; init; }

    //public required DirectionsApi DirectionsApi { get; init; }

    //public required MapsApi MapsApi { get; init; }

    public required PlacesApi PlacesApi { get; init; }

    //public required RoutesApi RoutesApi { get; init; }
}
