using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Json.Shared;

/// <summary>
/// Reference Route.
/// A supported reference route on the ComputeRoutesRequest.
/// </summary>
public enum ReferenceRoute
{
    /// <summary>
    /// Fuel efficient route.
    /// Routes labeled with this value are determined to be optimized for parameters such as fuel consumption.
    /// </summary>
    [EnumMember(Value = "FUEL_EFFICIENT")]
    FuelEfficient
}
