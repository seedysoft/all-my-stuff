namespace Seedysoft.Libs.GasStationPrices.Models.Minetur;

public record class ProductoPetrolifero : IComparable<ProductoPetrolifero>
{
    [J("IDProducto"), K(typeof(Core.Extensions.ParseStringConverter))]
    public required Constants.ProductoPetroliferoId IdProducto { get; init; }

    [J("NombreProducto")]
    public required string Nombre { get; init; }

    [J("NombreProductoAbreviatura")]
    public required string Abreviatura { get; init; }

    public int CompareTo(ProductoPetrolifero? other) => Abreviatura.CompareTo(other?.Abreviatura);

    public static System.Collections.Immutable.ImmutableSortedSet<ProductoPetrolifero> All
        => System.Collections.Immutable.ImmutableSortedSet.Create([BIE, BIO, G95E10, G95E5, G95E5Plus, G98E10, G98E5, GLP, GNC, GNL, GOA, GOAPlus, GOB/*, H2*/]);
    public static System.Collections.Immutable.ImmutableSortedSet<ProductoPetrolifero> Gasoline
        => System.Collections.Immutable.ImmutableSortedSet.Create([G95E10, G95E5, G95E5Plus, G98E10, G98E5]);
    public static System.Collections.Immutable.ImmutableSortedSet<ProductoPetrolifero> Diesel
        => System.Collections.Immutable.ImmutableSortedSet.Create([BIO, GOA, GOAPlus, GOB]);

    public override string ToString() => Nombre ?? "Unknown";

    #pragma warning disable format
    public static readonly ProductoPetrolifero BIE          = new() { IdProducto = Constants.ProductoPetroliferoId.BIE,         Nombre = "Bioetanol",                   Abreviatura =    nameof(BIE) };
    public static readonly ProductoPetrolifero BIO          = new() { IdProducto = Constants.ProductoPetroliferoId.BIO,         Nombre = "Biodiésel",                   Abreviatura =    nameof(BIO) };
    public static readonly ProductoPetrolifero G95E10       = new() { IdProducto = Constants.ProductoPetroliferoId.G95E10,      Nombre = "Gasolina 95 E10",             Abreviatura =    nameof(G95E10) };
    public static readonly ProductoPetrolifero G95E5        = new() { IdProducto = Constants.ProductoPetroliferoId.G95E5,       Nombre = "Gasolina 95 E5",              Abreviatura =    nameof(G95E5) };
    public static readonly ProductoPetrolifero G95E5Plus    = new() { IdProducto = Constants.ProductoPetroliferoId.G95E5Plus,   Nombre = "Gasolina 95 E5 Premium",      Abreviatura = $"{nameof(G95E5)}+" };
    public static readonly ProductoPetrolifero G98E10       = new() { IdProducto = Constants.ProductoPetroliferoId.G98E10,      Nombre = "Gasolina 98 E10",             Abreviatura =    nameof(G98E10) };
    public static readonly ProductoPetrolifero G98E5        = new() { IdProducto = Constants.ProductoPetroliferoId.G98E5,       Nombre = "Gasolina 98 E5",              Abreviatura =    nameof(G98E5) };
    public static readonly ProductoPetrolifero GLP          = new() { IdProducto = Constants.ProductoPetroliferoId.GLP,         Nombre = "Gases licuados del petróleo", Abreviatura =    nameof(GLP) };
    public static readonly ProductoPetrolifero GNC          = new() { IdProducto = Constants.ProductoPetroliferoId.GNC,         Nombre = "Gas natural comprimido",      Abreviatura =    nameof(GNC) };
    public static readonly ProductoPetrolifero GNL          = new() { IdProducto = Constants.ProductoPetroliferoId.GNL,         Nombre = "Gas natural licuado",         Abreviatura =    nameof(GNL) };
    public static readonly ProductoPetrolifero GOA          = new() { IdProducto = Constants.ProductoPetroliferoId.GOA,         Nombre = "Gasóleo A habitual",          Abreviatura =    nameof(GOA) };
    public static readonly ProductoPetrolifero GOAPlus      = new() { IdProducto = Constants.ProductoPetroliferoId.GOAPlus,     Nombre = "Gasóleo Premium",             Abreviatura = $"{nameof(GOA)}+" };
    public static readonly ProductoPetrolifero GOB          = new() { IdProducto = Constants.ProductoPetroliferoId.GOB,         Nombre = "Gasóleo B",                   Abreviatura =    nameof(GOB) };
    //public static readonly ProductoPetrolifero H2           = new() { IdProducto = Constants.ProductoPetroliferoId.H2,          Nombre = "Hidrógeno",                   Abreviatura =    nameof(H2) };
    #pragma warning restore format
}
