using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class GasStationModel
{
    public double Lat { get; set; }
    public double Lng { get; set; }

    public required string Rotulo { get; init; }

    public required IEnumerable<ProductPrice> Prices { get; init; }

    internal static GasStationModel Map(Models.Minetur.EstacionTerrestre estacionTerrestre) => new()
    {
        Lat = estacionTerrestre.Lat,
        Lng = estacionTerrestre.Lng,
        Prices = MapPrices(estacionTerrestre),
        Rotulo = estacionTerrestre.Rotulo,
    };

    private static System.Collections.ObjectModel.ReadOnlyCollection<ProductPrice> MapPrices(
        Models.Minetur.EstacionTerrestre estacionTerrestre)
    {
        IEnumerable<ProductPrice> values = [
            new(Models.Minetur.ProductoPetrolifero.G95E5, estacionTerrestre.PrecioGasolina95E5.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.G95E10, estacionTerrestre.PrecioGasolina95E10.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.G95E5Plus, estacionTerrestre.PrecioGasolina95E5Premium.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.G98E5, estacionTerrestre.PrecioGasolina98E5.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.G98E10, estacionTerrestre.PrecioGasolina98E10.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.GOA, estacionTerrestre.PrecioGasoleoA.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.GOAPlus, estacionTerrestre.PrecioGasoleoPremium.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.GOB, estacionTerrestre.PrecioGasoleoB.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.BIE, estacionTerrestre.PrecioBioetanol.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.BIO, estacionTerrestre.PrecioBiodiesel.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.GLP, estacionTerrestre.PrecioGasesLicuadosDelPetróleo.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.GNC, estacionTerrestre.PrecioGasNaturalComprimido.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.GNL, estacionTerrestre.PrecioGasNaturalLicuado.ParseWithNumberFormatInfoES()),
            new(Models.Minetur.ProductoPetrolifero.H2, estacionTerrestre.PrecioHidrogeno.ParseWithNumberFormatInfoES()),
        ];

        return values.Where(static x => x.Price.HasValue).ToList().AsReadOnly();
    }
}
