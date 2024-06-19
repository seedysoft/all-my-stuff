using Seedysoft.Carburantes.CoreLib.Entities.Core;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Seedysoft.Carburantes.CoreLib.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ProductoPetrolifero : EntityBase
{
    public int IdProducto { get; set; }

    [Display(Description = "Nombre del producto", Name = "Producto")]
    public string NombreProducto { get; set; } = default!;

    [Display(Description = "Abreviatura del producto", Name = "Abreviatura")]
    public string NombreProductoAbreviatura { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreProducto} ({IdProducto}) @ {AtDate}";
}

public class ProductoPetroliferoEqualityComparer : IEqualityComparer<ProductoPetrolifero>
{
    public bool Equals(ProductoPetrolifero? x, ProductoPetrolifero? y) => x?.AtDate == y?.AtDate && x?.IdProducto == y?.IdProducto;
    public int GetHashCode([DisallowNull] ProductoPetrolifero obj) => throw new NotImplementedException();
}
