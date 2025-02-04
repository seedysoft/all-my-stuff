namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class GasStationModel
{
    public double Lat { get; init; }
    public double Lng { get; init; }

    public required string Rotulo { get; init; }
    public string RotuloTrimed => Rotulo.Trim();

    public decimal? BIE { get; init; } 
    public decimal? BIO { get; init; }
    public decimal? G95E10 { get; init; }
    public decimal? G95E5 { get; init; }
    public decimal? G95E5Plus { get; init; }
    public decimal? G98E10 { get; init; }
    public decimal? G98E5 { get; init; }
    public decimal? GLP  { get; init; } 
    public decimal? GNC  { get; init; } 
    public decimal? GNL  { get; init; } 
    public decimal? GOA { get; init; }
    public decimal? GOAPlus { get; init; }
    public decimal? GOB  { get; init; } 
    //public decimal? H2 { get; init; }
}
