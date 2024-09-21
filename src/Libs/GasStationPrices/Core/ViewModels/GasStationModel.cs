using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Core.ViewModels;

public record class GasStationModel
{
    public required GoogleMapsComponents.Maps.LatLngLiteral LatLon { get; init; }

    public required string Rotulo { get; init; }

    public required IEnumerable<ProductPrice> Prices { get; init; }

    internal static GasStationModel Map(Json.Minetur.EstacionTerrestre estacionTerrestre) => new()
    {
        LatLon = new() { Lat = estacionTerrestre.Lat, Lng = estacionTerrestre.Lon },
        Prices = MapPrices(estacionTerrestre),
        Rotulo = estacionTerrestre.Rotulo,
    };

    private static IEnumerable<ProductPrice> MapPrices(Json.Minetur.EstacionTerrestre estacionTerrestre)
    {
        IEnumerable<ProductPrice> values = [
            new(Json.Minetur.ProductoPetrolifero.G95E5, estacionTerrestre.PrecioGasolina95E5.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.G95E10, estacionTerrestre.PrecioGasolina95E10.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.G95E5Plus, estacionTerrestre.PrecioGasolina95E5Premium.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.G98E5, estacionTerrestre.PrecioGasolina98E5.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.G98E10, estacionTerrestre.PrecioGasolina98E10.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.GOA, estacionTerrestre.PrecioGasoleoA.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.GOAPlus, estacionTerrestre.PrecioGasoleoPremium.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.GOB, estacionTerrestre.PrecioGasoleoB.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.BIE, estacionTerrestre.PrecioBioetanol.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.BIO, estacionTerrestre.PrecioBiodiesel.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.GLP, estacionTerrestre.PrecioGasesLicuadosDelPetróleo.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.GNC, estacionTerrestre.PrecioGasNaturalComprimido.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.GNL, estacionTerrestre.PrecioGasNaturalLicuado.ParseWithNumberFormatInfoES()),
            new(Json.Minetur.ProductoPetrolifero.H2, estacionTerrestre.PrecioHidrogeno.ParseWithNumberFormatInfoES()),
        ];

        return values.Where(x => x.Price.HasValue);
    }
}
