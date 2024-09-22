namespace Seedysoft.Libs.GasStationPrices.Core.Settings;

public readonly record struct Minetur
{
    [J("Uris")] public Uris Uris { get; init; }
}
