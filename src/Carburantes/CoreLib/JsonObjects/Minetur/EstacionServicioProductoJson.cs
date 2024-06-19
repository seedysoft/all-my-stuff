namespace Seedysoft.Carburantes.CoreLib.JsonObjects.Minetur;

public record EstacionServicioProductoJson : EstacionServicioBaseJson
{
    public string PrecioProducto { get; set; } = default!;
}
