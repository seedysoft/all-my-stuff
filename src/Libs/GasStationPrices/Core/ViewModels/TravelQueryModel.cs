namespace Seedysoft.Libs.GasStationPrices.Core.ViewModels;

public record class TravelQueryModel
{
    public required string Origin { get; set; }

    public required string Destination { get; set; }

    public required int MaxDistanceInKm { get; set; }

    public required IReadOnlyCollection<long> PetroleumProductsSelectedIds { get; set; } = [];

    private GoogleMapsRazorClassLib.Directions.LatLngBoundsLiteral bounds = new()
    {
        East = Libs.Core.Constants.Earth.MaxLongitudeInDegrees,
        North = Libs.Core.Constants.Earth.MaxLatitudeInDegrees,
        South = Libs.Core.Constants.Earth.MinLatitudeInDegrees,
        West = Libs.Core.Constants.Earth.MinLongitudeInDegrees,
    };

    public GasStationModel? IsInsideBounds(Json.Minetur.EstacionTerrestre estacionTerrestre)
    {
        const int decimals = 5;

        double GasStationLat = Math.Round(estacionTerrestre.Lat, decimals);
        double GasStationLon = Math.Round(estacionTerrestre.Lon, decimals);

        return
            GasStationLat < (double)bounds.North &&
            GasStationLat > (double)bounds.South &&
            GasStationLon < (double)bounds.East &&
            GasStationLon > (double)bounds.West ? GasStationModel.Map(estacionTerrestre) : null;
    }

    public void SetBounds(GoogleMapsRazorClassLib.Directions.LatLngBoundsLiteral latLngBoundsLiteral)
        => bounds = latLngBoundsLiteral;
}
