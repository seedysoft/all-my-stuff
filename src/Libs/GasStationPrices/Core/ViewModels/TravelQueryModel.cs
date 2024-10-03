namespace Seedysoft.Libs.GasStationPrices.Core.ViewModels;

public record class TravelQueryModel
{
    public required string Origin { get; set; }

    public required string Destination { get; set; }

    public required int MaxDistanceInKm { get; set; }

    public required IReadOnlyCollection<long> PetroleumProductsSelectedIds { get; set; } = [];

    private GoogleMapsComponents.Maps.LatLngBoundsLiteral bounds = new()
    {
        East = Libs.Core.Constants.Earth.MaxLongitudeInDegrees,
        North = Libs.Core.Constants.Earth.MaxLatitudeInDegrees,
        South = Libs.Core.Constants.Earth.MinLatitudeInDegrees,
        West = Libs.Core.Constants.Earth.MinLongitudeInDegrees,
    };
    public GoogleMapsComponents.Maps.LatLngBoundsLiteral Bounds
    {
        get => bounds;
        set
        {
            if (value != null)
                bounds = value;
        }
    }

    public GasStationModel? IsInsideBounds(Json.Minetur.EstacionTerrestre estacionTerrestre)
    {
        const int decimals = 5;

        double GasStationLat = Math.Round(estacionTerrestre.Lat, decimals);
        double GasStationLon = Math.Round(estacionTerrestre.Lon, decimals);

        // TODO                         Expand
        //double N = Utils.Helpers.GeometricHelper.ExpandLatitude(Bounds.North, Bounds.West, MaxDistanceInKm);
        //double S = Utils.Helpers.GeometricHelper.ExpandLatitude(Bounds.South, Bounds.East, MaxDistanceInKm);
        //double E = Utils.Helpers.GeometricHelper.ExpandLongitude(Bounds.South, Bounds.East, MaxDistanceInKm);
        //double W = Utils.Helpers.GeometricHelper.ExpandLongitude(Bounds.North, Bounds.West, MaxDistanceInKm);
        double N = Bounds.North;
        double S = Bounds.South;
        double E = Bounds.East;
        double W = Bounds.West;

        return GasStationLat < N && GasStationLat > S && GasStationLon < E && GasStationLon > W
            ? GasStationModel.Map(estacionTerrestre)
            : null;
    }
}
