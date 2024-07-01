using System.Text.Json.Serialization;

namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.Minetur;

public abstract record EstacionServicioBaseJson
{
    [JsonPropertyName("C.P.")]
    public string CodigoPostal { get; set; } = default!;

    [JsonPropertyName("Dirección")]
    public string Direccion { get; set; } = default!;

    public string Horario { get; set; } = default!;

    public string Latitud { get; set; } = default!;

    public string Localidad { get; set; } = default!;

    [JsonPropertyName("Longitud (WGS84)")]
    public string LongitudWgs84 { get; set; } = default!;

    public string Margen { get; set; } = default!;

    public string Municipio { get; set; } = default!;

    public string Provincia { get; set; } = default!;

    [JsonPropertyName("Remisión")]
    public string Remision { get; set; } = default!;

    [JsonPropertyName("Rótulo")]
    public string Rotulo { get; set; } = default!;

    [JsonPropertyName("Tipo Venta")]
    public string TipoVenta { get; set; } = default!;

    public string IDEESS { get; set; } = default!;

    public string IDMunicipio { get; set; } = default!;

    public string IDProvincia { get; set; } = default!;

    public string IDCCAA { get; set; } = default!;
}
