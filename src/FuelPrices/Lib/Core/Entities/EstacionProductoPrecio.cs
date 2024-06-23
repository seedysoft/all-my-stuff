namespace Seedysoft.FuelPrices.Lib.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class EstacionProductoPrecio : Core.EntityBase
{
    public int IdEstacion { get; set; }

    public int IdProducto { get; set; }

    public int CentimosDeEuro { get; set; }

    public decimal Euros => decimal.Divide(CentimosDeEuro, 1_000M);

    private string GetDebuggerDisplay() => $"Estación {IdEstacion}, Producto {IdProducto} y precio {Euros:0.000} @ {AtDate}";
}
