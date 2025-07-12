using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Extensions;

public static class ModelExtensions
{
    public static ViewModels.GasStationModel ToGasStationModel(this Models.Minetur.EstacionTerrestre estacionTerrestre)
    {
        return new()
        {
            #pragma warning disable format

            Lat             =   estacionTerrestre.Lat,
            Lng             =   estacionTerrestre.Lng,
            Localizacion    =   estacionTerrestre.DireccionParsed,
            Rotulo          =   estacionTerrestre.Rotulo,
            BIE             =   estacionTerrestre.PrecioBioetanol.ParseWithNumberFormatInfoES(),
            BIO             =   estacionTerrestre.PrecioBiodiesel.ParseWithNumberFormatInfoES(),
            GOB             =   estacionTerrestre.PrecioGasoleoB.ParseWithNumberFormatInfoES(),
            G95E10          =   estacionTerrestre.PrecioGasolina95E10.ParseWithNumberFormatInfoES(),
            G95E5           =   estacionTerrestre.PrecioGasolina95E5.ParseWithNumberFormatInfoES(),
            G95E5Plus       =   estacionTerrestre.PrecioGasolina95E5Premium.ParseWithNumberFormatInfoES(),
            G98E10          =   estacionTerrestre.PrecioGasolina98E10.ParseWithNumberFormatInfoES(),
            G98E5           =   estacionTerrestre.PrecioGasolina98E5.ParseWithNumberFormatInfoES(),
            GLP             =   estacionTerrestre.PrecioGasesLicuadosDelPetróleo.ParseWithNumberFormatInfoES(),
            GNC             =   estacionTerrestre.PrecioGasNaturalComprimido.ParseWithNumberFormatInfoES(),
            GNL             =   estacionTerrestre.PrecioGasNaturalLicuado.ParseWithNumberFormatInfoES(),
            GOA             =   estacionTerrestre.PrecioGasoleoA.ParseWithNumberFormatInfoES(),
            GOAPlus         =   estacionTerrestre.PrecioGasoleoPremium.ParseWithNumberFormatInfoES(),
            //H2              =   estacionTerrestre.PrecioHidrogeno.ParseWithNumberFormatInfoES(),

            #pragma warning restore format
        };
    }
}
