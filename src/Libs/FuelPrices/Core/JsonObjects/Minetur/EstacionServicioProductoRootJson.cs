using System.Text.Json.Serialization;

namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.Minetur;

public record EstacionServicioProductoRootJson : EstacionServicioBaseRootJson
{
    [JsonPropertyName("ListaEESSPrecio")]
    public EstacionServicioProductoJson[] EstacionServicioProducto { get; set; } = default!;
}
