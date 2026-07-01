namespace Seedysoft.Libs.GasStationPrices.Settings;

public readonly record struct GasStationPricesSettings
{
    public required Minetur Minetur { get; init; }
}

/// <summary>
/// For help visit https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/help
/// </summary>
public readonly record struct Minetur
{
    public required Urls Urls { get; init; }
}

public readonly record struct Urls
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
