namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class TravelQueryModel
{
    public required string Origin { get; set; }

    public required string Destination { get; set; }

    public required int MaxDistanceInKm { get; set; }

    public required IReadOnlyCollection<long> PetroleumProductsSelectedIds { get; set; } = [];

    public GoogleApis.Json.Shared.LatLngBoundsLiteral Bounds { get; set; } = new();

    public GasStationModel? IsInsideBounds(Json.Minetur.EstacionTerrestre estacionTerrestre)
    {
        // TODO     Filtrar por los productos seleccionados.
        // TODO             ¿Cómo "mapear" ProductosPetroliferos con las propiedades? ¿Switch?
        const int decimals = 5;

        double GasStationLat = Math.Round(estacionTerrestre.Lat, decimals);
        double GasStationLon = Math.Round(estacionTerrestre.Lon, decimals);

        return
            GasStationLat < Bounds.North &&
            GasStationLat > Bounds.South &&
            GasStationLon < Bounds.East &&
            GasStationLon > Bounds.West ? GasStationModel.Map(estacionTerrestre) : null;
    }
}
