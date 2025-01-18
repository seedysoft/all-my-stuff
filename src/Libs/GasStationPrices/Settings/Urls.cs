namespace Seedysoft.Libs.GasStationPrices.Settings;

public readonly record struct Urls
{
    [J("Base")]
    public string Base { get; init; }

    [J("EstacionesTerrestres")]
    public string EstacionesTerrestres { get; init; }

    [J("EstacionesTerrestresFiltroProducto")]
    public string EstacionesTerrestresFiltroProducto { get; init; }

    [J("ListadosBase")]
    public string ListadosBase { get; init; }
}
