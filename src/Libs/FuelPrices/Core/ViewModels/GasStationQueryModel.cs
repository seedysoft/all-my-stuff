using System.ComponentModel.DataAnnotations;

namespace Seedysoft.Libs.FuelPrices.Core.ViewModels;

public record GasStationQueryModel
{
    [Range(0.5, 50)]
    [Required]
    public decimal MaxDistanceInKm { get; set; } = default!;
    public double MaxDistanceInMeters => (double)MaxDistanceInKm * 1_000D;

    public IEnumerable<int> PetroleumProductsSelectedIds { get; set; } = [];

    public StepObtainedModel[] Steps { get; set; } = default!;
}
