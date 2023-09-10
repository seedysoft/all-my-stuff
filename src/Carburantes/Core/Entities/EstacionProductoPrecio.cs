﻿using System.Diagnostics;

namespace Seedysoft.Carburantes.Core.Entities;

public abstract class EstacionProductoPrecioBase : Core.EntityBase
{
    public int IdEstacion { get; set; }

    public int IdProducto { get; set; }

    public int CentimosDeEuro { get; set; }

    public decimal Euros => decimal.Divide(CentimosDeEuro, 1_000M);
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class EstacionProductoPrecio : EstacionProductoPrecioBase
{
    private string GetDebuggerDisplay() => $"Estación {IdEstacion}, Producto {IdProducto} y precio {Euros:0.000} @ {AtDate}";
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class EstacionProductoPrecioHist : EstacionProductoPrecioBase
{
    private string GetDebuggerDisplay() => $"Estación {IdEstacion}, Producto {IdProducto} y precio {Euros:0.000} @ {AtDate}";
}
