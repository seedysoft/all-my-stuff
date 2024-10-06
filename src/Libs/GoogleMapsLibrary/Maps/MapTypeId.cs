namespace GoogleMapsLibrary.Maps;

/// <summary>
/// Identifiers for common MapTypes.
/// Specify these by value, or by using the constant's name. For example, 'satellite' or google.maps.MapTypeId.SATELLITE.
/// </summary>
/// <see href="https://developers.google.com/maps/documentation/javascript/reference/map#MapTypeId"/>
public enum MapTypeId
{
    /// <summary>
    /// This map type displays a transparent layer of major streets on satellite images.
    /// </summary>
    [EnumMember(Value = "hybrid")]
    Hybrid,

    /// <summary>
    /// This map type displays a normal street map.
    /// </summary>
    //ROADMAP,
    [EnumMember(Value = "roadmap")]
    Roadmap,

    /// <summary>
    /// This map type displays satellite images.
    /// </summary>
    [EnumMember(Value = "satellite")]
    Satellite,

    /// <summary>
    /// This map type displays maps with physical features such as terrain and vegetation.
    /// </summary>
    [EnumMember(Value = "terrain")]
    Terrain
}
