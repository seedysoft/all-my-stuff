namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class GasStationModel
{
    public double Lat { get; init; }
    public double Lng { get; init; }

    public string? Localizacion { get; init; }

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

    public ProductAndPrice GetProductNameAndPrice(Constants.ProductoPetroliferoId productId)
    {
        return productId switch
        {
            #pragma warning disable format
            
            Constants.ProductoPetroliferoId.BIE         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.BIE.Nombre,       $"{BIE          :0.000 €}"),
            Constants.ProductoPetroliferoId.BIO         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.BIO.Nombre,       $"{BIO          :0.000 €}"),
            Constants.ProductoPetroliferoId.G95E10      => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G95E10.Nombre,    $"{G95E10       :0.000 €}"),
            Constants.ProductoPetroliferoId.G95E5       => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G95E5.Nombre,     $"{G95E5        :0.000 €}"),
            Constants.ProductoPetroliferoId.G95E5Plus   => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G95E5Plus.Nombre, $"{G95E5Plus    :0.000 €}"),
            Constants.ProductoPetroliferoId.G98E10      => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G98E10.Nombre,    $"{G98E10       :0.000 €}"),
            Constants.ProductoPetroliferoId.G98E5       => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G98E5.Nombre,     $"{G98E5        :0.000 €}"),
            Constants.ProductoPetroliferoId.GLP         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GLP.Nombre,       $"{GLP          :0.000 €}"),
            Constants.ProductoPetroliferoId.GNC         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GNC.Nombre,       $"{GNC          :0.000 €}"),
            Constants.ProductoPetroliferoId.GNL         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GNL.Nombre,       $"{GNL          :0.000 €}"),
            Constants.ProductoPetroliferoId.GOA         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GOA.Nombre,       $"{GOA          :0.000 €}"),
            Constants.ProductoPetroliferoId.GOAPlus     => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GOAPlus.Nombre,   $"{GOAPlus      :0.000 €}"),
            Constants.ProductoPetroliferoId.GOB         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GOB.Nombre,       $"{GOB          :0.000 €}"),
            //Constants.ProductoPetroliferoId.H2          => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.H2.Nombre,        $"{H2           :0.000 €}"),

            #pragma warning restore format

            _ => throw new ArgumentOutOfRangeException(nameof(productId), productId, "Not recognized"),
        };
    }
}
