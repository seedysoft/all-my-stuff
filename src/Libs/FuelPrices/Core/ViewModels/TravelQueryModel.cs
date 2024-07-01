using System.ComponentModel.DataAnnotations;

namespace Seedysoft.Libs.FuelPrices.Core.ViewModels;

public record TravelQueryModel
{
    [Required(AllowEmptyStrings = false)]
    public string Origin { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    public string Destination { get; set; } = default!;
}
