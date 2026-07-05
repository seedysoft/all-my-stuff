namespace Seedysoft.Libs.GasStationPrices.Settings;

public record class GasStationPricesSettings
{
    public required Minetur Minetur { get; init; }
}

/// <summary>
/// For help visit https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/help
/// </summary>
public record class Minetur
{
    public required Urls Urls { get; init; }
}

public record class Urls
{
    public required string Base { get; init; }

    public required string EstacionesTerrestres { get; init; }

    public required string EstacionesTerrestresFiltroProducto { get; init; }

    public required string ListadosBase { get; init; }
}
