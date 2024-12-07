namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// Encapsulates a set of optional conditions to satisfy when calculating the routes.
/// </summary>
public class RouteModifiers
{
    /// <summary>
    /// When set to true, avoids toll roads where reasonable, giving preference to routes not containing toll roads.
    /// Applies only to the <see cref="RouteTravelMode.DRIVE"/> and <see cref="RouteTravelMode.TwoWheeler"/>.
    /// </summary>
    [J("avoidTolls")]
    public bool AvoidTolls { get; set; }

    /// <summary>
    /// When set to true, avoids highways where reasonable, giving preference to routes not containing highways.
    /// Applies only to the <see cref="RouteTravelMode.DRIVE"/> and <see cref="RouteTravelMode.TwoWheeler"/>.
    /// </summary>
    [J("avoidHighways")]
    public bool AvoidHighways { get; set; }

    /// <summary>
    /// When set to true, avoids ferries where reasonable, giving preference to routes not containing ferries.
    /// Applies only to the <see cref="RouteTravelMode.DRIVE"/> and <see cref="RouteTravelMode.TwoWheeler"/>.
    /// </summary>
    [J("avoidFerries")]
    public bool AvoidFerries { get; set; }

    /// <summary>
    /// When set to true, avoids navigating indoors where reasonable, giving preference to routes not containing indoor navigation.
    /// Applies only to the <see cref="RouteTravelMode.Walk"/>.
    /// </summary>
    [J("avoidIndoor")]
    public bool AvoidIndoor { get; set; }

    /// <summary>
    /// Specifies the vehicle information.
    /// </summary>
    [J("vehicleInfo"), I(Condition = C.WhenWritingNull)]
    public VehicleInfo? VehicleInfo { get; set; }

    /// <summary>
    /// Encapsulates information about toll passes. If toll passes are provided, the API tries to return the pass price.
    /// If toll passes are not provided, the API treats the toll pass as unknown and tries to return the cash price.
    /// Applies only to the <see cref="RouteTravelMode.DRIVE"/> and <see cref="RouteTravelMode.TwoWheeler"/>.
    /// </summary>
    [J("tollPasses"), I(Condition = C.WhenWritingNull)]
    public TollPass[]? TollPasses { get; set; }
}
