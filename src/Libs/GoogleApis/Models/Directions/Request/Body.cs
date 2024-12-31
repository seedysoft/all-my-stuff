using OneOf;

namespace Seedysoft.Libs.GoogleApis.Models.Directions.Request;

/// <summary>
/// A directions query to be sent to the <see cref="Service"/>.
/// </summary>
public class Body
{
    /// <summary>
    /// Location of destination.
    /// This can be specified as either a string to be geocoded, or a LatLng, or a Place. 
    /// Required.
    /// </summary>
    [K(typeof(Serialization.OneOfJsonConverterFactory))]
    public required OneOf<string, Models.Shared.LatLngLiteral, Place> Destination { get; set; }

    /// <summary>
    /// Location of origin. 
    /// This can be specified as either a string to be geocoded, or a LatLng, or a Place. 
    /// Required.
    /// </summary>
    [K(typeof(Serialization.OneOfJsonConverterFactory))]
    public required OneOf<string, Models.Shared.LatLngLiteral, Place> Origin { get; set; }

    /// <summary>
    /// Type of routing requested. 
    /// Required.
    /// </summary>
    [K(typeof(Core.Extensions.EnumMemberJsonConverter<Shared.TravelMode>))]
    public required Shared.TravelMode TravelMode { get; set; }

    ///// <summary>
    ///// If true, instructs the Directions service to avoid ferries where possible. 
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public bool? AvoidFerries { get; set; }

    ///// <summary>
    ///// If true, instructs the Directions service to avoid highways where possible. 
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public bool? AvoidHighways { get; set; }

    ///// <summary>
    ///// If true, instructs the Directions service to avoid toll roads where possible. 
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public bool? AvoidTolls { get; set; }

    ///// <summary>
    ///// Settings that apply only to requests where travelMode is DRIVING. 
    ///// This object will have no effect for other travel modes.
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public DrivingOptions? DrivingOptions { get => TravelMode == TravelMode.Driving ? drivingOptions : null; set => drivingOptions = value; }
    //private DrivingOptions? drivingOptions;

    ///// <summary>
    ///// If set to true, the DirectionsService will attempt to re-order the supplied intermediate waypoints to minimize overall cost of the route. 
    ///// If waypoints are optimized, inspect DirectionsRoute.waypoint_order in the response to determine the new ordering.
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public bool? OptimizeWaypoints { get; set; }

    /// <summary>
    /// Whether or not route alternatives should be provided. 
    /// Optional.
    /// </summary>
    [I(Condition = C.WhenWritingNull)]
    public bool? ProvideRouteAlternatives { get; set; }

    ///// <summary>
    ///// Region code used as a bias for geocoding requests. 
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public string? Region { get; set; }

    ///// <summary>
    ///// Settings that apply only to requests where travelMode is TRANSIT. 
    ///// This object will have no effect for other travel modes.
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public TransitOptions? TransitOptions { get => TravelMode == TravelMode.Transit ? transitOptions : null; set => transitOptions = value; }
    //private TransitOptions? transitOptions;

    ///// <summary>
    ///// Preferred unit system to use when displaying distance. 
    ///// Defaults to the unit system used in the country of origin.
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //[DO_NOT_USE]
    //public UnitSystem? UnitSystem { get; set; }

    ///// <summary>
    ///// Array of intermediate waypoints.
    ///// Directions are calculated from the origin to the destination by way of each waypoint in this array. 
    ///// See the developer's guide for the maximum number of waypoints allowed. 
    ///// Waypoints are not supported for transit directions.
    ///// Optional.
    ///// </summary>
    //[I(Condition = C.WhenWritingNull)]
    //public IEnumerable<Waypoint>? Waypoints { get => TravelMode == TravelMode.Transit ? null : waypoints; set => waypoints = value; }
    //private IEnumerable<Waypoint>? waypoints;
}
