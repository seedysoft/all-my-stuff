using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.Core.JsonObjects.Minetur;

public record EstacionesServicioJson : EstacionServicioBaseJson
{
    [JsonPropertyName("Precio Biodiesel")]
    public string PrecioBiodiesel { get; set; } = default!;

    [JsonPropertyName("Precio Bioetanol")]
    public string PrecioBioetanol { get; set; } = default!;

    [JsonPropertyName("Precio Gas Natural Comprimido")]
    public string PrecioGasNaturalComprimido { get; set; } = default!;

    [JsonPropertyName("Precio Gas Natural Licuado")]
    public string PrecioGasNaturalLicuado { get; set; } = default!;

    [JsonPropertyName("Precio Gases licuados del petróleo")]
    public string PrecioGaseslicuadosdelpetróleo { get; set; } = default!;

    [JsonPropertyName("Precio Gasoleo A")]
    public string PrecioGasoleoA { get; set; } = default!;

    [JsonPropertyName("Precio Gasoleo B")]
    public string PrecioGasoleoB { get; set; } = default!;

    [JsonPropertyName("Precio Gasoleo Premium")]
    public string PrecioGasoleoPremium { get; set; } = default!;

    [JsonPropertyName("Precio Gasolina 95 E10")]
    public string PrecioGasolina95E10 { get; set; } = default!;

    [JsonPropertyName("Precio Gasolina 95 E5")]
    public string PrecioGasolina95E5 { get; set; } = default!;

    [JsonPropertyName("Precio Gasolina 95 E5 Premium")]
    public string PrecioGasolina95E5Premium { get; set; } = default!;

    [JsonPropertyName("Precio Gasolina 98 E10")]
    public string PrecioGasolina98E10 { get; set; } = default!;

    [JsonPropertyName("Precio Gasolina 98 E5")]
    public string PrecioGasolina98E5 { get; set; } = default!;

    [JsonPropertyName("Precio Hidrogeno")]
    public string PrecioHidrogeno { get; set; } = default!;

    [JsonPropertyName("% BioEtanol")]
    public string BioEtanol { get; set; } = default!;

    [JsonPropertyName("% Éster metílico")]
    public string Éstermetílico { get; set; } = default!;
}
