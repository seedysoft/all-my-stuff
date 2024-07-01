namespace Seedysoft.Libs.FuelPrices.Core.Settings;

public record Uris
{
    public required string Base { get; init; }

    public required string EstacionesTerrestres { get; init; }
    public required string EstacionesTerrestresFiltroProducto { get; init; }

    public required string ListadosBase { get; init; }
}
