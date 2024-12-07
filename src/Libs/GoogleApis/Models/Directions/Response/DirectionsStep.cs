//namespace Seedysoft.Libs.GoogleApis.Models.Directions.Response;

///// <summary>
///// A single DirectionsStep in a DirectionsResult. 
///// Some fields may be undefined. 
///// Note that though this object is "JSON-like," it is not strictly JSON, as it directly includes LatLng objects.
///// </summary>
//public class DirectionsStep
//{
//    /// <summary>
//    /// The distance covered by this step. 
//    /// This property may be undefined as the distance may be unknown.
//    /// </summary>
//    public Distance? Distance { get; set; }

//    /// <summary>
//    /// The typical time required to perform this step in seconds and in text form. 
//    /// This property may be undefined as the duration may be unknown.
//    /// </summary>
//    public Duration? Duration { get; set; }

//    /// <summary>
//    /// The ending location of this step.
//    /// </summary>
//    public Models.Shared.LatLngLiteral? EndLocation { get; set; }

//    /// <summary>
//    /// Instructions for this step.
//    /// </summary>
//    public string? Instructions { get; set; }

//    /// <summary>
//    /// The starting location of this step.
//    /// </summary>
//    public Models.Shared.LatLngLiteral? StartLocation { get; set; }

//    /// <summary>
//    /// Sub-steps of this step. 
//    /// Specified for non-transit sections of transit routes.
//    /// </summary>
//    public IEnumerable<DirectionsStep>? Steps { get; set; }

//    /// <summary>
//    /// Transit-specific details about this step. 
//    /// This property will be undefined unless the travel mode of this step is TRANSIT.
//    /// </summary>
//    public DirectionsTransitDetails? Transit { get; set; }

//    /// <summary>
//    /// The mode of travel used in this step.
//    /// </summary>
//    public Shared.TravelMode TravelMode { get; set; }

//    [J("lat_lngs")]
//    public IEnumerable<Models.Shared.LatLngLiteral>? LatLngs { get; set; }

//    /// <summary>
//    /// A sequence of LatLngs describing the course of this step.
//    /// </summary>
//    [J("path")]
//    public IEnumerable<Models.Shared.LatLngLiteral>? Path { get; set; }
//}
