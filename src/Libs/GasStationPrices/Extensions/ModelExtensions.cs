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
            bie             =   estacionTerrestre.PrecioBioetanol.ParseWithNumberFormatInfoES(),
            bio             =   estacionTerrestre.PrecioBiodiesel.ParseWithNumberFormatInfoES(),
            gob             =   estacionTerrestre.PrecioGasoleoB.ParseWithNumberFormatInfoES(),
            g95e10          =   estacionTerrestre.PrecioGasolina95E10.ParseWithNumberFormatInfoES(),
            g95e5           =   estacionTerrestre.PrecioGasolina95E5.ParseWithNumberFormatInfoES(),
            g95e5plus       =   estacionTerrestre.PrecioGasolina95E5Premium.ParseWithNumberFormatInfoES(),
            g98e10          =   estacionTerrestre.PrecioGasolina98E10.ParseWithNumberFormatInfoES(),
            g98e5           =   estacionTerrestre.PrecioGasolina98E5.ParseWithNumberFormatInfoES(),
            glp             =   estacionTerrestre.PrecioGasesLicuadosDelPetróleo.ParseWithNumberFormatInfoES(),
            gnc             =   estacionTerrestre.PrecioGasNaturalComprimido.ParseWithNumberFormatInfoES(),
            gnl             =   estacionTerrestre.PrecioGasNaturalLicuado.ParseWithNumberFormatInfoES(),
            goa             =   estacionTerrestre.PrecioGasoleoA.ParseWithNumberFormatInfoES(),
            goaplus         =   estacionTerrestre.PrecioGasoleoPremium.ParseWithNumberFormatInfoES(),
            //h2              =   estacionTerrestre.PrecioHidrogeno.ParseWithNumberFormatInfoES(),
#pragma warning restore format
        };
    }
}
