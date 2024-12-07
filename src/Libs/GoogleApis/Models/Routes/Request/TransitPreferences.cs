namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// Preferences for TRANSIT based routes that influence the route that is returned.
/// </summary>
public class TransitPreferences
{
    /// <summary>
    /// A set of travel modes to use when getting a TRANSIT route.
    /// Defaults to all supported modes of travel.
    /// </summary>
    [J("allowedTravelModes"), I(Condition = C.WhenWritingDefault), K(typeof(Utils.Extensions.EnumMemberJsonConverter<TransitTravelMode>))]
    public TransitTravelMode AllowedTravelModes { get; set; }

    /// <summary>
    /// A routing preference that, when specified, influences the TRANSIT route returned.
    /// </summary>
    [J("routingPreference"), I(Condition = C.WhenWritingDefault), K(typeof(Utils.Extensions.EnumMemberJsonConverter<TransitRoutingPreference>))]
    public TransitRoutingPreference RoutingPreference { get; set; }
}
