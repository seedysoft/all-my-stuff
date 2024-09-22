namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// A set of values that specify the unit of measure used in the display.
/// </summary>
public enum Units
{
    /// <summary>
    /// Metric units of measure.
    /// </summary>
    METRIC,
    /// <summary>
    /// Imperial (English) units of measure.
    /// </summary>
    IMPERIAL,
    /// <summary>
    /// Units of measure not specified. Defaults to the unit of measure inferred from the request.
    /// </summary>
    UNITS_UNSPECIFIED,
}
