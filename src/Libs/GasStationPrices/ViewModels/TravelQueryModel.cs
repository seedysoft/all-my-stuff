namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class TravelQueryModel
{
    public required string Origin { get; set; }

    public required string Destination { get; set; }

    public required int MaxDistanceInKm { get; set; }

    public required IReadOnlyCollection<long> PetroleumProductsSelectedIds { get; set; } = [];
}
