using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Extensions;

public static class ModelExtensions
{
    public static ViewModels.GasStationModel ToGasStationModel(this Models.Minetur.EstacionTerrestre estacionTerrestre)
    {
        return new()
        {
#pragma warning disable format
            Lat             =   (double)estacionTerrestre.Lat,
            Lon             =   (double)estacionTerrestre.Lon,
            Localizacion    =   estacionTerrestre.DireccionParsed,
            Rotulo          =   estacionTerrestre.Rotulo,
            Bie             =   estacionTerrestre.PrecioBioetanol.ParseWithNumberFormatInfoES(),
            Bio             =   estacionTerrestre.PrecioBiodiesel.ParseWithNumberFormatInfoES(),
            Gob             =   estacionTerrestre.PrecioGasoleoB.ParseWithNumberFormatInfoES(),
            G95e10          =   estacionTerrestre.PrecioGasolina95E10.ParseWithNumberFormatInfoES(),
            G95e5           =   estacionTerrestre.PrecioGasolina95E5.ParseWithNumberFormatInfoES(),
            G95e5plus       =   estacionTerrestre.PrecioGasolina95E5Premium.ParseWithNumberFormatInfoES(),
            G98e10          =   estacionTerrestre.PrecioGasolina98E10.ParseWithNumberFormatInfoES(),
            G98e5           =   estacionTerrestre.PrecioGasolina98E5.ParseWithNumberFormatInfoES(),
            Glp             =   estacionTerrestre.PrecioGasesLicuadosDelPetróleo.ParseWithNumberFormatInfoES(),
            Gnc             =   estacionTerrestre.PrecioGasNaturalComprimido.ParseWithNumberFormatInfoES(),
            Gnl             =   estacionTerrestre.PrecioGasNaturalLicuado.ParseWithNumberFormatInfoES(),
            Goa             =   estacionTerrestre.PrecioGasoleoA.ParseWithNumberFormatInfoES(),
            Goaplus         =   estacionTerrestre.PrecioGasoleoPremium.ParseWithNumberFormatInfoES(),
            //h2              =   estacionTerrestre.PrecioHidrogeno.ParseWithNumberFormatInfoES(),
#pragma warning restore format
        };
    }
}
