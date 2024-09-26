namespace Seedysoft.Libs.GasStationPrices.Core.Settings;

public record class Maps
{
    private string mapId = default!;
    public required string MapId
    {
        get => mapId;
        init => mapId = Cryptography.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }
    
    public required string UriFormat { get; init; }
}
