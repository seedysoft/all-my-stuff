namespace Seedysoft.Libs.GoogleApis.Json.Routes.Request;

public class Body
{
    /// <summary>
    /// Required. Origin waypoint.
    /// </summary>
    [J("origin")] public required Waypoint Origin { get; set; }

    /// <summary>
    /// Required. Destination waypoint.
    /// </summary>
    [J("destination")] public required Waypoint Destination { get; set; }

    /// <summary>
    /// Optional. A set of waypoints along the route (excluding terminal points), for either stopping at or passing by.
    /// Up to 25 intermediate waypoints are supported.
    /// </summary>
    [J("intermediates"), I(Condition = C.WhenWritingNull)] public Waypoint[]? Intermediates { get; set; }

    /// <summary>
    /// Optional. Specifies the mode of transportation.
    /// </summary>
    [J("travelMode"), I(Condition = C.WhenWritingNull), K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Shared.RouteTravelMode? TravelMode { get; set; }

    /// <summary>
    /// Optional. Specifies how to compute the route. The server attempts to use the selected routing preference to compute the route.
    /// If the routing preference results in an error or an extra long latency, then an error is returned.
    /// You can specify this option only when the <see cref="TravelMode"/> is <see cref="RouteTravelMode.DRIVE"/> or <see cref="RouteTravelMode.TWO_WHEELER"/>, otherwise the request fails.
    /// </summary>
    [J("routingPreference"), I(Condition = C.WhenWritingNull), K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Shared.RoutingPreference? RoutingPreference { get; set; }

    /// <summary>
    /// Optional. Specifies your preference for the quality of the polyline.
    /// </summary>
    [J("polylineQuality"), I(Condition = C.WhenWritingNull), K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Shared.PolylineQuality? PolylineQuality { get; set; }

    /// <summary>
    /// Optional. Specifies the preferred encoding for the polyline.
    /// </summary>
    [J("polylineEncoding"), I(Condition = C.WhenWritingNull), K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Shared.PolylineEncoding? PolylineEncoding { get; set; }

    /// <summary>
    /// Optional. The departure time. If you don't set this value, then this value defaults to the time that you made the request.
    /// NOTE: You can only specify a departureTime in the past when RouteTravelMode is set to TRANSIT.
    /// ransit trips are available for up to 7 days in the past or 100 days in the future.<br />
    /// A timestamp in RFC3339 UTC "Zulu" format, with nanosecond resolution and up to nine fractional digits.
    /// Examples: "2014-10-02T15:01:23Z" and "2014-10-02T15:01:23.045123456Z".
    /// </summary>
    [J("departureTime"), I(Condition = C.WhenWritingNull)] public DateTimeOffset? DepartureTime { get; set; }

    /// <summary>
    /// Optional. The arrival time. NOTE: Can only be set when RouteTravelMode is set to TRANSIT.
    /// You can specify either departureTime or arrivalTime, but not both.
    /// Transit trips are available for up to 7 days in the past or 100 days in the future.<br />
    /// A timestamp in RFC3339 UTC "Zulu" format, with nanosecond resolution and up to nine fractional digits.
    /// Examples: "2014-10-02T15:01:23Z" and "2014-10-02T15:01:23.045123456Z".
    /// </summary>
    [J("arrivalTime"), I(Condition = C.WhenWritingNull)] public DateTimeOffset? ArrivalTime { get; set; }

    /// <summary>
    /// Optional. Specifies whether to calculate alternate routes in addition to the route.
    /// No alternative routes are returned for requests that have intermediate waypoints.
    /// </summary>
    [J("computeAlternativeRoutes")] public bool ComputeAlternativeRoutes { get; set; }

    /// <summary>
    /// Optional. A set of conditions to satisfy that affect the way routes are calculated.
    /// </summary>
    [J("routeModifiers"), I(Condition = C.WhenWritingNull)] public RouteModifiers? RouteModifiers { get; set; }

    /// <summary>
    /// Optional. The BCP-47 language code, such as "en-US" or "sr-Latn".
    /// For more information, see <see href="http://www.unicode.org/reports/tr35/#Unicode_locale_identifier">Unicode Locale Identifier</see>.
    /// See <see href="https://developers.google.com/maps/faq#languagesupport">Language Support</see> for the list of supported languages.
    /// When you don't provide this value, the display language is inferred from the location of the route request.
    /// </summary>
    [J("languageCode"), I(Condition = C.WhenWritingNull)] public string? LanguageCode { get; set; }

    /// <summary>
    /// Optional. The region code, specified as a ccTLD ("top-level domain") two-character value.
    /// For more information see <see href="https://en.wikipedia.org/wiki/List_of_Internet_top-level_domains#Country_code_top-level_domains">Country code top-level domains</see>.
    /// </summary>
    [J("regionCode"), I(Condition = C.WhenWritingNull)] public string? RegionCode { get; set; }

    /// <summary>
    /// Optional. Specifies the units of measure for the display fields.
    /// These fields include the instruction field in <see href="https://developers.google.com/maps/documentation/routes/reference/rest/v2/TopLevel/computeRoutes#NavigationInstruction">NavigationInstruction</see>.
    /// The units of measure used for the route, leg, step distance, and duration are not affected by this value.
    /// If you don't provide this value, then the display units are inferred from the location of the first origin.
    /// </summary>
    [J("units"), I(Condition = C.WhenWritingNull), K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Shared.Units? Units { get; set; }

    /// <summary>
    /// Optional. If set to true, the service attempts to minimize the overall cost of the route by re-ordering the specified intermediate waypoints.
    /// The request fails if any of the intermediate waypoints is a via waypoint.
    /// Use ComputeRoutesResponse.Routes.optimized_intermediate_waypoint_index to find the new ordering.
    /// If ComputeRoutesResponseroutes.optimized_intermediate_waypoint_index is not requested in the X-Goog-FieldMask header, the request fails.
    /// If optimizeWaypointOrder is set to false, ComputeRoutesResponse.optimized_intermediate_waypoint_index will be empty.
    /// </summary>
    [J("optimizeWaypointOrder"), I(Condition = C.WhenWritingNull)] public bool? OptimizeWaypointOrder { get; set; }

    /// <summary>
    /// Optional. Specifies what reference routes to calculate as part of the request in addition to the default route.
    /// A reference route is a route with a different route calculation objective than the default route.
    /// For example a FUEL_EFFICIENT reference route calculation takes into account various parameters that would generate an optimal fuel efficient route.
    /// </summary>
    [J("requestedReferenceRoutes"), I(Condition = C.WhenWritingNull), K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Shared.ReferenceRoute? RequestedReferenceRoutes { get; set; }

    /// <summary>
    /// Optional. A list of extra computations which may be used to complete the request.
    /// Note: These extra computations may return extra fields on the response.
    /// These extra fields must also be specified in the field mask to be returned in the response.
    /// </summary>
    [J("extraComputations"), I(Condition = C.WhenWritingNull)] public Shared.ExtraComputation[]? ExtraComputations { get; set; }

    /// <summary>
    /// Optional. Specifies the assumptions to use when calculating time in traffic.
    /// This setting affects the value returned in the duration field in the <see cref="Response.Route"/> and <see cref="Response.RouteLeg"/> which contains the predicted time in traffic based on historical averages.
    /// <see cref="TrafficModel"/> is only available for requests that have set <see cref="RoutingPreference"/> to <see cref="RoutingPreference.TRAFFIC_AWARE_OPTIMAL"/> and <see cref="RouteTravelMode"/> to <see cref="RouteTravelMode.DRIVE"/>.
    /// Defaults to <see cref="TrafficModel.BEST_GUESS"/> if traffic is requested and <see cref="TrafficModel"/> is not specified.
    /// </summary>
    [J("trafficModel"), I(Condition = C.WhenWritingNull), K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Shared.TrafficModel? TrafficModel { get; set; }

    /// <summary>
    /// Optional. Specifies preferences that influence the route returned for <see cref="RouteTravelMode.TRANSIT"/> routes.
    /// NOTE: You can only specify a transitPreferences when <see cref="RouteTravelMode"/> is set to <see cref="RouteTravelMode.TRANSIT"/>.
    /// </summary>
    [J("transitPreferences"), I(Condition = C.WhenWritingNull)] public Shared.TransitPreferences? TransitPreferences { get; set; }
}
