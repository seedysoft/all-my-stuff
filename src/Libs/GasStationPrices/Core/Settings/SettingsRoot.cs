namespace Seedysoft.Libs.GasStationPrices.Core.Settings;

public record class SettingsRoot
{
    public required GoogleMapsPlatform GoogleMapsPlatform { get; init; }

    public required Minetur Minetur { get; init; }
}
