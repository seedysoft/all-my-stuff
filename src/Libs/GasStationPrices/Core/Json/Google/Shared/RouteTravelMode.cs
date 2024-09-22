namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

/// <summary>
/// A set of values used to specify the mode of travel.
/// NOTE: <see cref="WALK"/>, <see cref="BICYCLE"/>, and <see cref="TWO_WHEELER"/> routes are in beta and might sometimes be missing clear sidewalks, pedestrian paths, or bicycling paths.
/// You must display this warning to the user for all walking, bicycling, and two-wheel routes that you display in your app.
/// </summary>
public enum RouteTravelMode
{
    /// <summary>
    /// Travel by passenger car.
    /// </summary>
    DRIVE,
    /// <summary>
    /// Travel by bicycle.
    /// </summary>
    BICYCLE,
    /// <summary>
    /// Travel by walking.
    /// </summary>
    WALK,
    /// <summary>
    /// Two-wheeled, motorized vehicle. For example, motorcycle.
    /// Note that this differs from the <see cref="BICYCLE"/> travel mode which covers human-powered mode.
    /// </summary>
    TWO_WHEELER,
    /// <summary>
    /// Travel by public transit routes, where available.
    /// </summary>
    TRANSIT,
    /// <summary>
    /// No travel mode specified. Defaults to <see cref="Drive"/>.
    /// </summary>
    TRAVEL_MODE_UNSPECIFIED = DRIVE,
};
