namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// A supported reference route on the ComputeRoutesRequest.
/// </summary>
public enum ReferenceRoute
{
    /// <summary>
    /// Not used. Requests containing this value fail.
    /// </summary>
    REFERENCE_ROUTE_UNSPECIFIED,
    /// <summary>
    /// Fuel efficient route. Routes labeled with this value are determined to be optimized for parameters such as fuel consumption.
    /// </summary>
    FUEL_EFFICIENT,
}
