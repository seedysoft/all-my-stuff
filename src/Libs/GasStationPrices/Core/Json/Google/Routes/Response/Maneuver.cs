namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// A set of values that specify the navigation action to take for the current step (for example, turn left, merge, or straight).
/// </summary>
public enum Maneuver
{
    /// <summary>
    /// Not used.
    /// </summary>
    MANEUVER_UNSPECIFIED,
    /// <summary>
    /// Turn slightly to the left.
    /// </summary>
    TURN_SLIGHT_LEFT,
    /// <summary>
    /// Turn sharply to the left.
    /// </summary>
    TURN_SHARP_LEFT,
    /// <summary>
    /// Make a left u-turn.
    /// </summary>
    UTURN_LEFT,
    /// <summary>
    /// Turn left.
    /// </summary>
    TURN_LEFT,
    /// <summary>
    /// Turn slightly to the right.
    /// </summary>
    TURN_SLIGHT_RIGHT,
    /// <summary>
    /// Turn sharply to the right.
    /// </summary>
    TURN_SHARP_RIGHT,
    /// <summary>
    /// Make a right u-turn.
    /// </summary>
    UTURN_RIGHT,
    /// <summary>
    /// Turn right.
    /// </summary>
    TURN_RIGHT,
    /// <summary>
    /// Go straight.
    /// </summary>
    STRAIGHT,
    /// <summary>
    /// Take the left ramp.
    /// </summary>
    RAMP_LEFT,
    /// <summary>
    /// Take the right ramp.
    /// </summary>
    RAMP_RIGHT,
    /// <summary>
    /// Merge into traffic.
    /// </summary>
    MERGE,
    /// <summary>
    /// Take the left fork.
    /// </summary>
    FORK_LEFT,
    /// <summary>
    /// Take the right fork.
    /// </summary>
    FORK_RIGHT,
    /// <summary>
    /// Take the ferry.
    /// </summary>
    FERRY,
    /// <summary>
    /// Take the train leading onto the ferry.
    /// </summary>
    FERRY_TRAIN,
    /// <summary>
    /// Turn left at the roundabout.
    /// </summary>
    ROUNDABOUT_LEFT,
    /// <summary>
    /// Turn right at the roundabout.
    /// </summary>
    ROUNDABOUT_RIGHT,
    /// <summary>
    /// Initial maneuver.
    /// </summary>
    DEPART,
    /// <summary>
    /// Used to indicate a street name change.
    /// </summary>
    NAME_CHANGE,
}
