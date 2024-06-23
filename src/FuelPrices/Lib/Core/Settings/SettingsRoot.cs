namespace Seedysoft.FuelPrices.Lib.Core.Settings;

public record SettingsRoot
{
    public required GoogleMapsPlatform GoogleMapsPlatform { get; init; }

    public required Minetur Minetur { get; init; }

    public required ObtainDataSettings ObtainDataSettings { get; init; }
}
