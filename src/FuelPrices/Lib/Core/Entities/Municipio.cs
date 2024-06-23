using System.Diagnostics;

namespace Seedysoft.FuelPrices.Lib.Core.Entities;

public abstract class MunicipioBase : Core.EntityBase
{
    public int IdMunicipio { get; set; }

    public int IdProvincia { get; set; }

    public string NombreMunicipio { get; set; } = default!;
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class Municipio : MunicipioBase
{
    private string GetDebuggerDisplay() => $"{NombreMunicipio}({IdMunicipio}({IdProvincia}))";
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class MunicipioHist : MunicipioBase
{
    private string GetDebuggerDisplay() => $"{NombreMunicipio} ({IdMunicipio}({IdProvincia})) @ {AtDate}";
}
