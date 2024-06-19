using Seedysoft.Carburantes.CoreLib.Entities.Core;

namespace Seedysoft.Carburantes.CoreLib.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ComunidadAutonoma : EntityBase
{
    public int IdComunidadAutonoma { get; set; }

    public string NombreComunidadAutonoma { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreComunidadAutonoma} ({IdComunidadAutonoma}) @ {AtDate}";
}
