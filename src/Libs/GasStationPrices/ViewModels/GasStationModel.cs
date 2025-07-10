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
    public decimal? GLP { get; init; }
    public decimal? GNC { get; init; }
    public decimal? GNL { get; init; }
    public decimal? GOA { get; init; }
    public decimal? GOAPlus { get; init; }
    public decimal? GOB { get; init; }
    //public decimal? H2 { get; init; }

    public decimal? GetProductPrice(Constants.ProductoPetroliferoId productId)
    {
        return productId switch
        {
            #pragma warning disable format
            Constants.ProductoPetroliferoId.BIE         => BIE,
            Constants.ProductoPetroliferoId.BIO         => BIO,
            Constants.ProductoPetroliferoId.G95E10      => G95E10,
            Constants.ProductoPetroliferoId.G95E5       => G95E5,
            Constants.ProductoPetroliferoId.G95E5Plus   => G95E5Plus,
            Constants.ProductoPetroliferoId.G98E10      => G98E10,
            Constants.ProductoPetroliferoId.G98E5       => G98E5,
            Constants.ProductoPetroliferoId.GLP         => GLP,
            Constants.ProductoPetroliferoId.GNC         => GNC,
            Constants.ProductoPetroliferoId.GNL         => GNL,
            Constants.ProductoPetroliferoId.GOA         => GOA,
            Constants.ProductoPetroliferoId.GOAPlus     => GOAPlus,
            Constants.ProductoPetroliferoId.GOB         => GOB,
            //Constants.ProductoPetroliferoId.H2          => H2;
            _                                           => null,
            #pragma warning restore format
        };
    }
}
