namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record GasStationModel
{
    [J("lat")] public double Lat { get; init; }
    [J("lng")] public double Lon { get; init; }

    // TODO                         Use Travel.Models.Location instead individual properties
    public Travel.Models.Location Location => new((decimal)Lat, (decimal)Lon);

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

    public decimal? GetProdById(Constants.ProductoPetroliferoId productoPetroliferoId)
    {
        return productoPetroliferoId switch
        {
            Constants.ProductoPetroliferoId.BIE => bie,
            Constants.ProductoPetroliferoId.BIO => bio,
            Constants.ProductoPetroliferoId.G95E10 => g95e10,
            Constants.ProductoPetroliferoId.G95E5 => g95e5,
            Constants.ProductoPetroliferoId.G95E5Plus => g95e5plus,
            Constants.ProductoPetroliferoId.G98E10 => g98e10,
            Constants.ProductoPetroliferoId.G98E5 => g98e5,
            Constants.ProductoPetroliferoId.GLP => glp,
            Constants.ProductoPetroliferoId.GNC => gnc,
            Constants.ProductoPetroliferoId.GNL => gnl,
            Constants.ProductoPetroliferoId.GOA => goa,
            Constants.ProductoPetroliferoId.GOAPlus => goaplus,
            Constants.ProductoPetroliferoId.GOB => gob,
            _ => throw new ArgumentOutOfRangeException(nameof(productoPetroliferoId), productoPetroliferoId, null),
        };
    }

    public IReadOnlyList<(Constants.ProductoPetroliferoId IdProducto, decimal Value)> AllProducts()
    {
        return [..
            from a in Models.Minetur.ProductoPetrolifero.All
            let b = GetProdById(a.IdProducto)
            where b.HasValue
            select (a.IdProducto, b.Value)
        ];
    }
}
