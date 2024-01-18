namespace Seedysoft.Carburantes.Core.Settings;

public record Minetur
{
    public Uris Uris { get; init; } = default!;
}

public record Uris
{
    public string Base { get; init; } = default!;

    public string EstacionesTerrestres { get; init; } = default!;
    public string EstacionesTerrestresFiltroProducto { get; init; } = default!;

    public string ListadosBase { get; init; } = default!;
}
