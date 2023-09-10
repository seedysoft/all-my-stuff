using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.Core.JsonObjects.Minetur;

public record EstacionesServicioRootJson : EstacionServicioBaseRootJson
{
    [JsonPropertyName("ListaEESSPrecio")]
    public EstacionesServicioJson[] Estaciones { get; set; } = default!;
}
