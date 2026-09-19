namespace Seedysoft.Libs.GasStationPrices.Models.Minetur;

public readonly record struct ProductoPetrolifero : IComparable<ProductoPetrolifero>
{
    [J("IDProducto")] public required Constants.ProductoPetroliferoId IdProducto { get; init; }

    [J("NombreProducto")] public required string Nombre { get; init; }

    [J("NombreProductoAbreviatura")] public required string Abreviatura { get; init; }

    public int CompareTo(ProductoPetrolifero other) => Abreviatura.CompareTo(other.Abreviatura);

    public static System.Collections.Immutable.ImmutableSortedSet<ProductoPetrolifero> Available
        => System.Collections.Immutable.ImmutableSortedSet.Create([ADB, DREN, G95E10, G95E25, G95E5, G95E5Plus, G95E85, G98E10, G98E5, GOA, GOAPlus, GOB, GREN]);
    public static System.Collections.Immutable.ImmutableSortedSet<ProductoPetrolifero> Gasoline
        => System.Collections.Immutable.ImmutableSortedSet.Create([G95E10, G95E25, G95E5, G95E5Plus, G95E85, G98E10, G98E5, GREN]);
    public static System.Collections.Immutable.ImmutableSortedSet<ProductoPetrolifero> Diesel
        => System.Collections.Immutable.ImmutableSortedSet.Create([ADB, BIO, DREN, GOA, GOAPlus, GOB]);

    public override string ToString() => Nombre ?? "Unknown";

    public static readonly ProductoPetrolifero ADB = new() { IdProducto = Constants.ProductoPetroliferoId.ADB, Nombre = "Adblue", Abreviatura = nameof(ADB).ToLowerInvariant() };
    public static readonly ProductoPetrolifero AMO = new() { IdProducto = Constants.ProductoPetroliferoId.AMO, Nombre = "Amoniaco", Abreviatura = nameof(AMO).ToLowerInvariant() };
    public static readonly ProductoPetrolifero BGNC = new() { IdProducto = Constants.ProductoPetroliferoId.BGNC, Nombre = "Biogas natural comprimido", Abreviatura = nameof(BGNC).ToLowerInvariant() };
    public static readonly ProductoPetrolifero BGNL = new() { IdProducto = Constants.ProductoPetroliferoId.BGNL, Nombre = "Biogas natural licuado", Abreviatura = nameof(BGNL).ToLowerInvariant() };
    public static readonly ProductoPetrolifero BIE = new() { IdProducto = Constants.ProductoPetroliferoId.BIE, Nombre = "Bioetanol", Abreviatura = nameof(BIE).ToLowerInvariant() };
    public static readonly ProductoPetrolifero BIO = new() { IdProducto = Constants.ProductoPetroliferoId.BIO, Nombre = "Biodiésel", Abreviatura = nameof(BIO).ToLowerInvariant() };
    public static readonly ProductoPetrolifero DREN = new() { IdProducto = Constants.ProductoPetroliferoId.DREN, Nombre = "Diésel renovable", Abreviatura = nameof(DREN).ToLowerInvariant() };
    public static readonly ProductoPetrolifero FOB = new() { IdProducto = Constants.ProductoPetroliferoId.FOB, Nombre = "Fuelóleo bajo índice azufre", Abreviatura = nameof(FOB).ToLowerInvariant() };
    public static readonly ProductoPetrolifero FOE = new() { IdProducto = Constants.ProductoPetroliferoId.FOE, Nombre = "Fuelóleo especial", Abreviatura = nameof(FOE).ToLowerInvariant() };
    public static readonly ProductoPetrolifero G95E10 = new() { IdProducto = Constants.ProductoPetroliferoId.G95E10, Nombre = "Gasolina 95 E10", Abreviatura = nameof(G95E10).ToLowerInvariant() };
    public static readonly ProductoPetrolifero G95E25 = new() { IdProducto = Constants.ProductoPetroliferoId.G95E25, Nombre = "Gasolina 95 E25", Abreviatura = nameof(G95E25).ToLowerInvariant() };
    public static readonly ProductoPetrolifero G95E5 = new() { IdProducto = Constants.ProductoPetroliferoId.G95E5, Nombre = "Gasolina 95 E5", Abreviatura = nameof(G95E5).ToLowerInvariant() };
    public static readonly ProductoPetrolifero G95E5Plus = new() { IdProducto = Constants.ProductoPetroliferoId.G95E5Plus, Nombre = "Gasolina 95 E5 Premium", Abreviatura = nameof(G95E5Plus).ToLowerInvariant() };
    public static readonly ProductoPetrolifero G95E85 = new() { IdProducto = Constants.ProductoPetroliferoId.G95E85, Nombre = "Gasolina 95 E85", Abreviatura = nameof(G95E85).ToLowerInvariant() };
    public static readonly ProductoPetrolifero G98E10 = new() { IdProducto = Constants.ProductoPetroliferoId.G98E10, Nombre = "Gasolina 98 E10", Abreviatura = nameof(G98E10).ToLowerInvariant() };
    public static readonly ProductoPetrolifero G98E5 = new() { IdProducto = Constants.ProductoPetroliferoId.G98E5, Nombre = "Gasolina 98 E5", Abreviatura = nameof(G98E5).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GLP = new() { IdProducto = Constants.ProductoPetroliferoId.GLP, Nombre = "Gases licuados del petróleo", Abreviatura = nameof(GLP).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GNAV = new() { IdProducto = Constants.ProductoPetroliferoId.GNAV, Nombre = "Gasolina de aviación", Abreviatura = nameof(GNAV).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GNC = new() { IdProducto = Constants.ProductoPetroliferoId.GNC, Nombre = "Gas natural comprimido", Abreviatura = nameof(GNC).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GNL = new() { IdProducto = Constants.ProductoPetroliferoId.GNL, Nombre = "Gas natural licuado", Abreviatura = nameof(GNL).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GOA = new() { IdProducto = Constants.ProductoPetroliferoId.GOA, Nombre = "Gasóleo A habitual", Abreviatura = nameof(GOA).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GOAPlus = new() { IdProducto = Constants.ProductoPetroliferoId.GOAPlus, Nombre = "Gasóleo Premium", Abreviatura = nameof(GOAPlus).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GOB = new() { IdProducto = Constants.ProductoPetroliferoId.GOB, Nombre = "Gasóleo B", Abreviatura = nameof(GOB).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GOC = new() { IdProducto = Constants.ProductoPetroliferoId.GOC, Nombre = "Gasóleo C", Abreviatura = nameof(GOC).ToLowerInvariant() };
    public static readonly ProductoPetrolifero GREN = new() { IdProducto = Constants.ProductoPetroliferoId.GREN, Nombre = "Gasolina renovable", Abreviatura = nameof(GREN).ToLowerInvariant() };
    public static readonly ProductoPetrolifero H2 = new() { IdProducto = Constants.ProductoPetroliferoId.H2, Nombre = "Hidrógeno", Abreviatura = nameof(H2).ToLowerInvariant() };
    public static readonly ProductoPetrolifero JETA1 = new() { IdProducto = Constants.ProductoPetroliferoId.JETA1, Nombre = "Queroseno de aviación JET_A1", Abreviatura = nameof(JETA1).ToLowerInvariant() };
    public static readonly ProductoPetrolifero JETA2 = new() { IdProducto = Constants.ProductoPetroliferoId.JETA2, Nombre = "Queroseno de aviación JET_A2", Abreviatura = nameof(JETA2).ToLowerInvariant() };
    public static readonly ProductoPetrolifero MET = new() { IdProducto = Constants.ProductoPetroliferoId.MET, Nombre = "Metanol", Abreviatura = nameof(MET).ToLowerInvariant() };
    public static readonly ProductoPetrolifero MGO = new() { IdProducto = Constants.ProductoPetroliferoId.MGO, Nombre = "Gasóleo para uso marítimo", Abreviatura = nameof(MGO).ToLowerInvariant() };
}
