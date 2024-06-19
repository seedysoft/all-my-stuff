using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.CoreLib.JsonObjects.Minetur;

public record EstacionServicioProductoRootJson : EstacionServicioBaseRootJson
{
    [JsonPropertyName("ListaEESSPrecio")]
    public EstacionServicioProductoJson[] EstacionServicioProducto { get; set; } = default!;
}
