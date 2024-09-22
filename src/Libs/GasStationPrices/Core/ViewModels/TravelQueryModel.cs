namespace Seedysoft.Libs.GasStationPrices.Core.ViewModels;

public record class TravelQueryModel
{
    public required string Origin { get; set; }

    public required string Destination { get; set; }

    public required decimal MaxDistanceInKm { get; set; }
    public double MaxDistanceInMeters => (double)MaxDistanceInKm * 1_000D;

    public required IReadOnlyCollection<long> PetroleumProductsSelectedIds { get; set; } = [];

    public GoogleMapsComponents.Maps.LatLngBoundsLiteral Bounds { get; } = new();
}
