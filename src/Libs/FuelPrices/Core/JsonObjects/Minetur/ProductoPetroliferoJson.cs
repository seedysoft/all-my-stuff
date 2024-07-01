namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.Minetur;

public record ProductoPetroliferoJson
{
    public string IDProducto { get; set; } = default!;
    public string NombreProducto { get; set; } = default!;
    public string NombreProductoAbreviatura { get; set; } = default!;
}
