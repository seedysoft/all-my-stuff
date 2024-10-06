namespace GoogleMapsLibrary.Core;

/// <summary>
/// Identifiers for map color schemes.
/// Specify these by value, or by using the constant's name.
/// For example, 'FOLLOW_SYSTEM' or google.maps.ColorScheme.FOLLOW_SYSTEM.
/// </summary>
/// <remarks>https://developers.google.com/maps/documentation/javascript/reference/map#ColorScheme</remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ColorScheme
{
    /// <summary>
    /// The dark color scheme for a map.
    /// </summary>
    [EnumMember(Value = "DARK")]
    Dark,

    /// <summary>
    /// The color scheme is selected based on system preferences.
    /// </summary>
    [EnumMember(Value = "FOLLOW_SYSTEM")]
    FollowSystem,

    /// <summary>
    /// The light color scheme for a map.
    /// Default value for legacy Maps JS.
    /// </summary>
    [EnumMember(Value = "LIGHT")]
    Light
}
