namespace Seedysoft.Carburantes.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class Provincia : Core.EntityBase
{
    public int IdProvincia { get; set; }

    public int IdComunidadAutonoma { get; set; }

    public string NombreProvincia { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreProvincia} ({IdProvincia}({IdComunidadAutonoma})) @ {AtDate}";
}
