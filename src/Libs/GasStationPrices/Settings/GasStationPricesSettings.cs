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

    public required string EstacionesTerrestresEndPoint { get; init; }

    public Uri GetUri() => new(Base + EstacionesTerrestresEndPoint);

    //public required string EstacionesTerrestresFiltroProductoQuery { get; init; }

    //public required string ListadosBaseEndPoint { get; init; }
}
