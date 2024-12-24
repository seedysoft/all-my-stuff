using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Shared;

/// <summary>
/// Route Travel mode.
/// </summary>
public enum RouteTravelMode
{
    /// <summary>
    /// No travel mode specified.
    /// Defaults to <see cref="Drive"/>.
    /// </summary>
    [EnumMember(Value = "TRAVEL_MODE_UNSPECIFIED")]
    TravelModeUnspecified,

    /// <summary>
    /// Travel by passenger car.
    /// </summary>
    [EnumMember(Value = "DRIVE")]
    Drive,

    /// <summary>
    /// Travel by bicycle.
    /// </summary>
    [EnumMember(Value = "BICYCLE")]
    Bicycle,

    /// <summary>
    /// Travel by walking.
    /// </summary>
    [EnumMember(Value = "WALK")]
    Walk,

    /// <summary>
    /// Two-wheeled, motorized vehicle.
    /// For example, motorcycle.
    /// Note that this differs from the <see cref="Bicycle"/> travel mode which covers human-powered mode.
    /// </summary>
    [EnumMember(Value = "TWO_WHEELER")]
    TwoWheeler,

    /// <summary>
    /// Travel by public transit routes, where available.
    /// </summary>
    [EnumMember(Value = "TRANSIT")]
    Transit,
}
