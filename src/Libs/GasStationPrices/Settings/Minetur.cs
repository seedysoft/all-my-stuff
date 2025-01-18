namespace Seedysoft.Libs.GasStationPrices.Settings;

/// <summary>
/// For help visit https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/help
/// </summary>
public record class Minetur
{
    public required Urls Urls { get; init; }
}
