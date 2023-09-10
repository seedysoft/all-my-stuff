using System.Diagnostics;

namespace Seedysoft.Carburantes.Core.Entities;

public abstract class ComunidadAutonomaBase : Core.EntityBase
{
    public int IdComunidadAutonoma { get; set; }

    public string NombreComunidadAutonoma { get; set; } = default!;
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class ComunidadAutonoma : ComunidadAutonomaBase
{
    private string GetDebuggerDisplay() => $"{NombreComunidadAutonoma} ({IdComunidadAutonoma})";
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class ComunidadAutonomaHist : ComunidadAutonomaBase
{
    private string GetDebuggerDisplay() => $"{NombreComunidadAutonoma} ({IdComunidadAutonoma}) @ {AtDate}";
}
