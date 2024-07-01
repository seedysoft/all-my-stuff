namespace Seedysoft.Libs.FuelPrices.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ProductoPetrolifero : Core.EntityBase
{
    public int IdProducto { get; set; }

    [System.ComponentModel.DataAnnotations.Display(Description = "Nombre del producto", Name = "Producto")]
    public string NombreProducto { get; set; } = default!;

    [System.ComponentModel.DataAnnotations.Display(Description = "Abreviatura del producto", Name = "Abreviatura")]
    public string NombreProductoAbreviatura { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreProducto} ({IdProducto}) @ {AtDate}";
}
