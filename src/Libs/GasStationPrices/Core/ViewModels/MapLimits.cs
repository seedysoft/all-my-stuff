namespace Seedysoft.Libs.GasStationPrices.Core.ViewModels;

public record MapLimits
{
    public required double North { get; set; }
    public required double South { get; set; }
    public required double East { get; set; }
    public required double West { get; set; }
}
