namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class GasStationModel
{
    [J("lat")] public double Lat { get; init; }
    [J("lng")] public double Lng { get; init; }

    // TODO                         Use Geocoding.Models.Location instead individual properties
    public Geocoding.Models.Location Location => new() { Latitude = Lat, Longitude = Lng, };

    [J("localizacion")] public string? Localizacion { get; init; }

    [J("rotulo")] public required string Rotulo { get; init; }
    [J("rotulotrimed")] public string RotuloTrimed => Rotulo.Trim();

#pragma warning disable IDE1006 // Naming Styles
    public decimal? bie { get; init; }
    public decimal? bio { get; init; }
    public decimal? g95e10 { get; init; }
    public decimal? g95e5 { get; init; }
    public decimal? g95e5plus { get; init; }
    public decimal? g98e10 { get; init; }
    public decimal? g98e5 { get; init; }
    public decimal? glp { get; init; }
    public decimal? gnc { get; init; }
    public decimal? gnl { get; init; }
    public decimal? goa { get; init; }
    public decimal? goaplus { get; init; }
    public decimal? gob { get; init; }
    public decimal? h2 { get; init; }
#pragma warning restore IDE1006 // Naming Styles

    public ProductAndPrice GetProductNameAndPrice(Constants.ProductoPetroliferoId productId)
    {
        return productId switch
        {
#pragma warning disable format
            Constants.ProductoPetroliferoId.BIE         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.BIE.Nombre,       $"{bie          :0.000 €}"),
            Constants.ProductoPetroliferoId.BIO         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.BIO.Nombre,       $"{bio          :0.000 €}"),
            Constants.ProductoPetroliferoId.G95E10      => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G95E10.Nombre,    $"{g95e10       :0.000 €}"),
            Constants.ProductoPetroliferoId.G95E5       => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G95E5.Nombre,     $"{g95e5        :0.000 €}"),
            Constants.ProductoPetroliferoId.G95E5Plus   => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G95E5Plus.Nombre, $"{g95e5plus    :0.000 €}"),
            Constants.ProductoPetroliferoId.G98E10      => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G98E10.Nombre,    $"{g98e10       :0.000 €}"),
            Constants.ProductoPetroliferoId.G98E5       => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.G98E5.Nombre,     $"{g98e5        :0.000 €}"),
            Constants.ProductoPetroliferoId.GLP         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GLP.Nombre,       $"{glp          :0.000 €}"),
            Constants.ProductoPetroliferoId.GNC         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GNC.Nombre,       $"{gnc          :0.000 €}"),
            Constants.ProductoPetroliferoId.GNL         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GNL.Nombre,       $"{gnl          :0.000 €}"),
            Constants.ProductoPetroliferoId.GOA         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GOA.Nombre,       $"{goa          :0.000 €}"),
            Constants.ProductoPetroliferoId.GOAPlus     => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GOAPlus.Nombre,   $"{goaplus      :0.000 €}"),
            Constants.ProductoPetroliferoId.GOB         => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.GOB.Nombre,       $"{gob          :0.000 €}"),
            //Constants.ProductoPetroliferoId.H2          => new ProductAndPrice(Models.Minetur.ProductoPetrolifero.H2.Nombre,        $"{h2           :0.000 €}"),
#pragma warning restore format

            _ => throw new ArgumentOutOfRangeException(nameof(productId), productId, "Not recognized"),
        };
    }
}
