using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.CoreLib.JsonObjects.Minetur;

public record EstacionesServicioRootJson : EstacionServicioBaseRootJson
{
    [JsonPropertyName("ListaEESSPrecio")]
    public EstacionesServicioJson[] Estaciones { get; set; } = default!;
}
