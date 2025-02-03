﻿using Seedysoft.Libs.Core.Extensions;
using System.Collections.Frozen;

namespace Seedysoft.Libs.GasStationPrices.Extensions;

public static class ModelExtensions
{
    public static ViewModels.GasStationModel ToGasStationModel(
        this Models.Minetur.EstacionTerrestre estacionTerrestre)
    {
        return new()
        {
            Lat = estacionTerrestre.Lat,
            Lng = estacionTerrestre.Lng,
            Prices = MapPrices(estacionTerrestre),
            Rotulo = estacionTerrestre.Rotulo,
        };

        static FrozenSet<ViewModels.ProductPrice> MapPrices(Models.Minetur.EstacionTerrestre estacionTerrestre)
        {
            IEnumerable<ViewModels.ProductPrice> values = [
                new(Models.Minetur.ProductoPetrolifero.BIE.IdProducto      , estacionTerrestre.PrecioBioetanol.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.BIO.IdProducto      , estacionTerrestre.PrecioBiodiesel.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.G95E10.IdProducto   , estacionTerrestre.PrecioGasolina95E10.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.G95E5.IdProducto    , estacionTerrestre.PrecioGasolina95E5.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.G95E5Plus.IdProducto, estacionTerrestre.PrecioGasolina95E5Premium.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.G98E10.IdProducto   , estacionTerrestre.PrecioGasolina98E10.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.G98E5.IdProducto    , estacionTerrestre.PrecioGasolina98E5.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.GLP.IdProducto      , estacionTerrestre.PrecioGasesLicuadosDelPetróleo.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.GNC.IdProducto      , estacionTerrestre.PrecioGasNaturalComprimido.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.GNL.IdProducto      , estacionTerrestre.PrecioGasNaturalLicuado.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.GOA.IdProducto      , estacionTerrestre.PrecioGasoleoA.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.GOAPlus.IdProducto  , estacionTerrestre.PrecioGasoleoPremium.ParseWithNumberFormatInfoES()),
                new(Models.Minetur.ProductoPetrolifero.GOB.IdProducto      , estacionTerrestre.PrecioGasoleoB.ParseWithNumberFormatInfoES()),
                //new(Models.Minetur.ProductoPetrolifero.H2.IdProducto       , estacionTerrestre.PrecioHidrogeno.ParseWithNumberFormatInfoES()),
            ];

            return values/*.Where(static x => x.Price.HasValue)*/.ToFrozenSet();
        }
    }
}
