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
    public required string Base { get; init; }

    public required string EstacionesTerrestres { get; init; }

    public Uri GetUri() => new(Base + EstacionesTerrestres);

    //public required string EstacionesTerrestresFiltroProducto { get; init; }

    //public required string ListadosBase { get; init; }
}
