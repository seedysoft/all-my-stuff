using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// A set of values describing the vehicle's emission type.
/// Applies only to the <see cref="Shared.RouteTravelMode.Drive"></see> <see cref="Shared.RouteTravelMode"></see>.
/// </summary>
public enum VehicleEmissionType
{
    /// <summary>
    /// No emission type specified.
    /// Default to GASOLINE.
    /// </summary>
    [EnumMember(Value = "VEHICLE_EMISSION_TYPE_UNSPECIFIED")]
    VehicleEmissionTypeUnspecified,

    /// <summary>
    /// Gasoline/petrol fueled vehicle.
    /// </summary>
    [EnumMember(Value = "GASOLINE")]
    Gasoline,

    /// <summary>
    /// Electricity powered vehicle.
    /// </summary>
    [EnumMember(Value = "ELECTRIC")]
    Electric,

    /// <summary>
    /// Hybrid fuel (such as gasoline + electric) vehicle.
    /// </summary>
    [EnumMember(Value = "HYBRID")]
    Hybrid,

    /// <summary>
    /// Diesel fueled vehicle.
    /// </summary>
    [EnumMember(Value = "DIESEL")]
    Diesel,
}
