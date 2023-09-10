namespace Seedysoft.Carburantes.Core.Settings;

public class Minetur
{
    public Uris Uris { get; set; } = default!;
}

public class Uris
{
    public string Base { get; set; } = default!;

    public string EstacionesTerrestres { get; set; } = default!;
    public string EstacionesTerrestresFiltroProducto { get; set; } = default!;

    public string ListadosBase { get; set; } = default!;
}