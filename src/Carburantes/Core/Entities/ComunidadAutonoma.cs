namespace Seedysoft.Carburantes.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ComunidadAutonoma : Core.EntityBase
{
    public int IdComunidadAutonoma { get; set; }

    public string NombreComunidadAutonoma { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreComunidadAutonoma} ({IdComunidadAutonoma}) @ {AtDate}";
}
