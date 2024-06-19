using Seedysoft.Carburantes.CoreLib.Entities.Core;

namespace Seedysoft.Carburantes.CoreLib.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class Provincia : EntityBase
{
    public int IdProvincia { get; set; }

    public int IdComunidadAutonoma { get; set; }

    public string NombreProvincia { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreProvincia} ({IdProvincia}({IdComunidadAutonoma})) @ {AtDate}";
}
