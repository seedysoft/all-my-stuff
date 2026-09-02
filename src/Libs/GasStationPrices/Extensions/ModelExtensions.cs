using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Extensions;

public static class ModelExtensions
{
    public static ViewModels.GasStationModel? ToGasStationModel(this Models.Minetur.EstacionTerrestre? estacionTerrestre)
    {
        return estacionTerrestre switch
        {
            null => null,
            _ => new()
            {
#pragma warning disable format
                Lat             =   (double)estacionTerrestre.Lat,
                Lon             =   (double)estacionTerrestre.Lon,
                Localizacion    =   estacionTerrestre.DireccionParsed,
                Rotulo          =   estacionTerrestre.Rotulo,
                Adb             =   estacionTerrestre.PrecioAdblue.ParseWithNumberFormatInfoES(),
                //Amo             =   estacionTerrestre.PrecioAmoniaco.ParseWithNumberFormatInfoES(),
                //Bgnc            =   estacionTerrestre.PrecioBiogasNaturalComprimido.ParseWithNumberFormatInfoES(),
                //Bgnl            =   estacionTerrestre.PrecioBiogasNaturalLicuado.ParseWithNumberFormatInfoES(),
                //Bie             =   estacionTerrestre.PrecioBioetanol.ParseWithNumberFormatInfoES(),
                Bio             =   estacionTerrestre.PrecioBiodiesel.ParseWithNumberFormatInfoES(),
                Dren            =   estacionTerrestre.PrecioGasolinaRenovable.ParseWithNumberFormatInfoES(),
                G95e10          =   estacionTerrestre.PrecioGasolina95E10.ParseWithNumberFormatInfoES(),
                G95e25          =   estacionTerrestre.PrecioGasolina95E25.ParseWithNumberFormatInfoES(),
                G95e5           =   estacionTerrestre.PrecioGasolina95E5.ParseWithNumberFormatInfoES(),
                G95e5plus       =   estacionTerrestre.PrecioGasolina95E5Premium.ParseWithNumberFormatInfoES(),
                G95e85          =   estacionTerrestre.PrecioGasolina95E85.ParseWithNumberFormatInfoES(),
                G98e10          =   estacionTerrestre.PrecioGasolina98E10.ParseWithNumberFormatInfoES(),
                G98e5           =   estacionTerrestre.PrecioGasolina98E5.ParseWithNumberFormatInfoES(),
                //Glp             =   estacionTerrestre.PrecioGasesLicuadosDelPetróleo.ParseWithNumberFormatInfoES(),
                //Gnc             =   estacionTerrestre.PrecioGasNaturalComprimido.ParseWithNumberFormatInfoES(),
                //Gnl             =   estacionTerrestre.PrecioGasNaturalLicuado.ParseWithNumberFormatInfoES(),
                Goa             =   estacionTerrestre.PrecioGasoleoA.ParseWithNumberFormatInfoES(),
                Goaplus         =   estacionTerrestre.PrecioGasoleoPremium.ParseWithNumberFormatInfoES(),
                Gob             =   estacionTerrestre.PrecioGasoleoB.ParseWithNumberFormatInfoES(),
                //H2              =   estacionTerrestre.PrecioHidrogeno.ParseWithNumberFormatInfoES(),
                //Met             =   estacionTerrestre.PrecioMetanol.ParseWithNumberFormatInfoES(),
#pragma warning restore format
            }
        };
    }
}
