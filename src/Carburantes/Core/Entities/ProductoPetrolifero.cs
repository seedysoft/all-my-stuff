﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Seedysoft.Carburantes.Core.Entities;

public abstract class ProductoPetroliferoBase : Core.EntityBase
{
    public int IdProducto { get; set; }

    [Display(Description = "Nombre del producto", Name = "Producto")]
    public string NombreProducto { get; set; } = default!;

    [Display(Description = "Abreviatura del producto", Name = "Abreviatura")]
    public string NombreProductoAbreviatura { get; set; } = default!;
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class ProductoPetrolifero : ProductoPetroliferoBase
{
    private string GetDebuggerDisplay() => $"{NombreProducto} ({IdProducto})";
}

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class ProductoPetroliferoHist : ProductoPetroliferoBase
{
    private string GetDebuggerDisplay() => $"{NombreProducto} ({IdProducto}) @ {AtDate}";
}
