﻿using System.ComponentModel.DataAnnotations;

namespace Seedysoft.FuelPrices.Lib.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ProductoPetrolifero : Core.EntityBase
{
    public int IdProducto { get; set; }

    [Display(Description = "Nombre del producto", Name = "Producto")]
    public string NombreProducto { get; set; } = default!;

    [Display(Description = "Abreviatura del producto", Name = "Abreviatura")]
    public string NombreProductoAbreviatura { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreProducto} ({IdProducto}) @ {AtDate}";
}
