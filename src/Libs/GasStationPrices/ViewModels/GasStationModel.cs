namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record GasStationModel
{
    [J("lat")] public double Lat { get; init; }
    [J("lng")] public double Lon { get; init; }

    public Travel.Models.Location Location => new(Lat, Lon);

    [J("localizacion")] public string? Localizacion { get; init; }

    [J("rotulo")] public required string Rotulo { get; init; }
    [J("rotulotrimed")] public string RotuloTrimed => Rotulo.Trim();

    [J("adb")] public decimal? Adb { get; init; }
    //[J("amo")] public decimal? Amo { get; init; }
    //[J("bie")] public decimal? Bie { get; init; }
    [J("bio")] public decimal? Bio { get; init; }
    //[J("bgnc")] public decimal? Bgnc { get; init; }
    //[J("bgnl")] public decimal? Bgnl { get; init; }
    [J("dren")] public decimal? Dren { get; init; }
    [J("g95e10")] public decimal? G95e10 { get; init; }
    [J("g95e25")] public decimal? G95e25 { get; init; }
    [J("g95e5")] public decimal? G95e5 { get; init; }
    [J("g95e5plus")] public decimal? G95e5plus { get; init; }
    [J("g95e85")] public decimal? G95e85 { get; init; }
    [J("g98e10")] public decimal? G98e10 { get; init; }
    [J("g98e5")] public decimal? G98e5 { get; init; }
    //[J("glp")] public decimal? Glp { get; init; }
    //[J("gnc")] public decimal? Gnc { get; init; }
    //[J("gnl")] public decimal? Gnl { get; init; }
    [J("goa")] public decimal? Goa { get; init; }
    [J("goaplus")] public decimal? Goaplus { get; init; }
    [J("gob")] public decimal? Gob { get; init; }
    [J("gren")] public decimal? Gren { get; init; }
    //[J("h2")] public decimal? H2 { get; init; }
    //[J("met")] public decimal? Met { get; init; }

    public decimal? GetProdById(Constants.ProductoPetroliferoId productoPetroliferoId)
    {
        return productoPetroliferoId switch
        {
            Constants.ProductoPetroliferoId.ADB => Adb,
            //Constants.ProductoPetroliferoId.AMO => Amo,
            //Constants.ProductoPetroliferoId.BGNC => Bgnc,
            //Constants.ProductoPetroliferoId.BGNL => Bgnl,
            //Constants.ProductoPetroliferoId.BIE => Bie,
            Constants.ProductoPetroliferoId.BIO => Bio,
            Constants.ProductoPetroliferoId.DREN => Dren,
            Constants.ProductoPetroliferoId.G95E10 => G95e10,
            Constants.ProductoPetroliferoId.G95E25 => G95e25,
            Constants.ProductoPetroliferoId.G95E5 => G95e5,
            Constants.ProductoPetroliferoId.G95E5Plus => G95e5plus,
            Constants.ProductoPetroliferoId.G95E85 => G95e85,
            Constants.ProductoPetroliferoId.G98E10 => G98e10,
            Constants.ProductoPetroliferoId.G98E5 => G98e5,
            //Constants.ProductoPetroliferoId.GLP => Glp,
            //Constants.ProductoPetroliferoId.GNC => Gnc,
            //Constants.ProductoPetroliferoId.GNL => Gnl,
            Constants.ProductoPetroliferoId.GOA => Goa,
            Constants.ProductoPetroliferoId.GOAPlus => Goaplus,
            Constants.ProductoPetroliferoId.GOB => Gob,
            Constants.ProductoPetroliferoId.GREN => Gren,
            //Constants.ProductoPetroliferoId.H2 => H2,
            //Constants.ProductoPetroliferoId.MET => Met,

            _ => throw new ArgumentOutOfRangeException(nameof(productoPetroliferoId), productoPetroliferoId, null),
        };
    }

    public IReadOnlyList<(Constants.ProductoPetroliferoId IdProducto, decimal Value)> AllProducts(
        IEnumerable<Constants.ProductoPetroliferoId>? petroleumProductsSelectedIds)
    {
        return [..
            from a in petroleumProductsSelectedIds ?? []
            let b = GetProdById(a)
            where b.HasValue
            select (a, b.Value)
        ];
    }
}
