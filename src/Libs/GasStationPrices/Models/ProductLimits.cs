namespace Seedysoft.Libs.GasStationPrices.Models;

public class ProductLimits(Constants.ProductoPetroliferoId idP, decimal? min = null, decimal? avg = null, decimal? max = null)
{
    public Constants.ProductoPetroliferoId IdP { get; private set; } = idP;
    public decimal? Min { get; private set; } = min;
    public decimal? Avg { get; private set; } = avg;
    public decimal? Max { get; private set; } = max;

    public string MinOrNull => FormatString(Min);
    public string AvgOrNull => FormatString(Avg);
    public string MaxOrNull => FormatString(Max);

    public void SetPrices(ProductLimits? from)
    {
        Min = from?.Min;
        Avg = from?.Avg;
        Max = from?.Max;
    }

    private static string FormatString(decimal? val) =>
        val.HasValue ? val.Value.ToString("0.000", Core.Constants.Globalization.NumberFormatInfoInvariant) : "null";

    public override string ToString() =>
        $"Id {$"{Minetur.ProductoPetrolifero.All.First(x => x.IdProducto == IdP).Abreviatura,10}"} ⬇ {MinOrNull} ~ {AvgOrNull} ⬆ {MaxOrNull}";
}
