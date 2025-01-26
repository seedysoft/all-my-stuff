namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class GasStationModel
{
    public double Lat { get; set; }
    public double Lng { get; set; }

    public required string Rotulo { get; init; }
    public string RotuloTrimed => Rotulo.Trim();

    public required System.Collections.Frozen.FrozenSet<ProductPrice> Prices { get; init; }
}
