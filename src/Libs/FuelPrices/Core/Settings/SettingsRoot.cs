namespace Seedysoft.Libs.FuelPrices.Core.Settings;

public record SettingsRoot
{
    public required GoogleMapsPlatform GoogleMapsPlatform { get; init; }

    public required Minetur Minetur { get; init; }
}
