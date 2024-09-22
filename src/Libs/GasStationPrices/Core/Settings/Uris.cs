namespace Seedysoft.Libs.GasStationPrices.Core.Settings;

public readonly record struct Uris
{
    [J("Base")] public Uri Base { get; init; }

    [J("EstacionesTerrestres")] public string EstacionesTerrestres { get; init; }

    [J("EstacionesTerrestresFiltroProducto")] public string EstacionesTerrestresFiltroProducto { get; init; }

    [J("ListadosBase")] public string ListadosBase { get; init; }
}
