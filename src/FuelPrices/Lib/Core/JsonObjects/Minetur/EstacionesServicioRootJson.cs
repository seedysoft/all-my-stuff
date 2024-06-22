using System.Text.Json.Serialization;

namespace Seedysoft.FuelPrices.Lib.Core.JsonObjects.Minetur;

public record EstacionesServicioRootJson : EstacionServicioBaseRootJson
{
    [JsonPropertyName("ListaEESSPrecio")]
    public EstacionesServicioJson[] Estaciones { get; set; } = default!;
}
