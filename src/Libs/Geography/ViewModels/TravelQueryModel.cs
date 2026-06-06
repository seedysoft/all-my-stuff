namespace Seedysoft.Libs.Geography.ViewModels;

public record class TravelQueryModel
{
    public required Place Origin { get; set; }

    public required Place Destination { get; set; }

    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IReadOnlyCollection<GasStationPrices.Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];
}
