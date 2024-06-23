using System.Text.Json.Serialization;

namespace Seedysoft.FuelPrices.Lib.Core.JsonObjects.Minetur;

public record EstacionServicioProductoRootJson : EstacionServicioBaseRootJson
{
    [JsonPropertyName("ListaEESSPrecio")]
    public EstacionServicioProductoJson[] EstacionServicioProducto { get; set; } = default!;
}
