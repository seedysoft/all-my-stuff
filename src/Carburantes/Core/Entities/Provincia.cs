﻿using System.Diagnostics;

namespace Seedysoft.Carburantes.Core.Entities;

public abstract class ProvinciaBase : Core.EntityBase
{
    public int IdProvincia { get; set; }

    public int IdComunidadAutonoma { get; set; }

    public string NombreProvincia { get; set; } = default!;
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class Provincia : ProvinciaBase
{
    private string GetDebuggerDisplay() => $"{NombreProvincia} ({IdProvincia}({IdComunidadAutonoma}))";
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class ProvinciaHist : ProvinciaBase
{
    private string GetDebuggerDisplay() => $"{NombreProvincia} ({IdProvincia}({IdComunidadAutonoma})) @ {AtDate}";
}
