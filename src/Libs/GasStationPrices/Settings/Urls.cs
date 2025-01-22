namespace Seedysoft.Libs.GasStationPrices.Settings;

public record Urls
{
    [J("Base")]
    public required string Base { get; init; }

    [J("EstacionesTerrestres")]
    public required string EstacionesTerrestres { get; init; }

    [J("EstacionesTerrestresFiltroProducto")]
    public required string EstacionesTerrestresFiltroProducto { get; init; }

    [J("ListadosBase")]
    public required string ListadosBase { get; init; }
}
