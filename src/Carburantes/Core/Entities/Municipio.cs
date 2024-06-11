namespace Seedysoft.Carburantes.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class Municipio : Core.EntityBase
{
    public int IdMunicipio { get; set; }

    public int IdProvincia { get; set; }

    public string NombreMunicipio { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreMunicipio} ({IdMunicipio}({IdProvincia})) @ {AtDate}";
}
