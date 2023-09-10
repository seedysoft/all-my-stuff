namespace Seedysoft.Carburantes.Core.JsonObjects.Minetur;

public record EstacionServicioProductoJson : EstacionServicioBaseJson
{
    public string PrecioProducto { get; set; } = default!;
}
