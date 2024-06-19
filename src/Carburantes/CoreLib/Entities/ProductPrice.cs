using Seedysoft.Carburantes.CoreLib.Entities.Core;
using System.ComponentModel.DataAnnotations;

namespace Seedysoft.Carburantes.CoreLib.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class ProductPrice : EntityBase
{
    public int CentimosDeEuro { get; set; }
    public decimal Euros => decimal.Divide(CentimosDeEuro, 1_000M);

    public string CodigoPostal { get; set; } = default!;

    public string Direccion { get; set; } = default!;

    public string Horario { get; set; } = default!;

    public int IdEstacion { get; set; }

    public int IdProducto { get; set; }

    public string Latitud { get; set; } = default!;

    public string Localidad { get; set; } = default!;

    public string LongitudWgs84 { get; set; } = default!;

    public string Margen { get; set; } = default!;

    public string NombreMunicipio { get; set; } = default!;

    [Display(Description = "Nombre del producto", Name = "Producto")]
    public string NombreProducto { get; set; } = default!;

    [Display(Description = "Abreviatura del producto", Name = "Abreviatura")]
    public string NombreProductoAbreviatura { get; set; } = default!;

    public string NombreProvincia { get; set; } = default!;

    public string Rotulo { get; set; } = default!;

    private string GetDebuggerDisplay() => $"{NombreProducto} @ {AtDate}";
}
