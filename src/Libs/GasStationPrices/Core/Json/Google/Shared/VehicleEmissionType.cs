namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

public enum VehicleEmissionType
{
    /// <summary>
    /// Gasoline/petrol fueled vehicle.
    /// </summary>
    GASOLINE,
    /// <summary>
    /// Electricity powered vehicle.
    /// </summary>
    ELECTRIC,
    /// <summary>
    /// Hybrid fuel (such as gasoline + electric) vehicle.
    /// </summary>
    HYBRID,
    /// <summary>
    /// Diesel fueled vehicle.
    /// </summary>
    DIESEL,
    /// <summary>
    /// No emission type specified. Default to <see cref="GASOLINE"/>.
    /// </summary>
    VEHICLE_EMISSION_TYPE_UNSPECIFIED = GASOLINE,
}
