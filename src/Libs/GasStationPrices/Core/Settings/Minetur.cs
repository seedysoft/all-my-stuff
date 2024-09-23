﻿namespace Seedysoft.Libs.GasStationPrices.Core.Settings;

/// <summary>
/// For help visit https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/help
/// </summary>
public record class Minetur
{
    public required Uris Uris { get; init; }
}
