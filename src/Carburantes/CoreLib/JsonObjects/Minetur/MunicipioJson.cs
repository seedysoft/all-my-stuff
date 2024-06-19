namespace Seedysoft.Carburantes.CoreLib.JsonObjects.Minetur;

public record MunicipioJson
{
    public string IDMunicipio { get; set; } = default!;
    public string IDProvincia { get; set; } = default!;
    public string IDCCAA { get; set; } = default!;
    public string Municipio { get; set; } = default!;
    public string Provincia { get; set; } = default!;
    public string CCAA { get; set; } = default!;
}
