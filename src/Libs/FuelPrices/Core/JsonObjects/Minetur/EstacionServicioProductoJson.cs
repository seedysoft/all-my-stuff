namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.Minetur;

public record EstacionServicioProductoJson : EstacionServicioBaseJson
{
    public string PrecioProducto { get; set; } = default!;
}
