namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.Minetur;

public record ProvinciaJson
{
    public string IDPovincia { get; set; } = default!;
    public string IDCCAA { get; set; } = default!;
    public string Provincia { get; set; } = default!;
    public string CCAA { get; set; } = default!;
}
