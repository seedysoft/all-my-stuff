using System.Diagnostics;

namespace Seedysoft.FuelPrices.Lib.Core.Entities;

public abstract class ComunidadAutonomaBase : Core.EntityBase
{
    public int IdComunidadAutonoma { get; set; }

    public string NombreComunidadAutonoma { get; set; } = default!;
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ComunidadAutonoma : ComunidadAutonomaBase
{
    private string GetDebuggerDisplay() => $"{NombreComunidadAutonoma} ({IdComunidadAutonoma})";
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ComunidadAutonomaHist : ComunidadAutonomaBase
{
    private string GetDebuggerDisplay() => $"{NombreComunidadAutonoma} ({IdComunidadAutonoma}) @ {AtDate}";
}
