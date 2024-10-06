namespace GoogleMapsLibrary.Marker;

/// <summary>
/// 
/// </summary>
/// <see href=""/>
public enum CollisionBehavior
{
    /// <summary>
    /// 
    /// </summary>
    [EnumMember(Value = "google.maps.CollisionBehavior.REQUIRED")]
    Required,

    /// <summary>
    /// 
    /// </summary>
    [EnumMember(Value = "google.maps.CollisionBehavior.REQUIRED_AND_HIDES_OPTIONAL")]
    RequiredAndHidesOptional,

    /// <summary>
    /// 
    /// </summary>
    [EnumMember(Value = "google.maps.CollisionBehavior.OPTIONAL_AND_HIDES_LOWER_PRIORITY")]
    OptionalAndHidesLowerPriority,
}
