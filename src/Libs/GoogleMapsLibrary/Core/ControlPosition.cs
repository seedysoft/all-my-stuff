namespace GoogleMapsLibrary.Core;

/// <summary>
/// Identifiers used to specify the placement of controls on the map.
/// Controls are positioned relative to other controls in the same layout position.
/// Controls that are added first are positioned closer to the edge of the map.
/// Usage of "logical values" <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_logical_properties_and_values"/>) is recommended in order to be able to automatically support both left-to-right (LTR) and right-to-left (RTL) layout contexts.
/// </summary>
/// <remarks>https://developers.google.com/maps/documentation/javascript/reference/control#ControlPosition</remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ControlPosition
{
    /// <summary>
    /// Equivalent to BOTTOM_CENTER in both LTR and RTL.
    /// </summary>
    [EnumMember(Value = "BLOCK_END_INLINE_CENTER")]
    BlockEndInlineCenter,

    /// <summary>
    /// Equivalent to BOTTOM_RIGHT in LTR, or BOTTOM_LEFT in RTL.
    /// </summary>
    [EnumMember(Value = "BLOCK_END_INLINE_END")]
    BlockEndInlineEnd,

    /// <summary>
    /// Equivalent to BOTTOM_LEFT in LTR, or BOTTOM_RIGHT in RTL.
    /// </summary>
    [EnumMember(Value = "BLOCK_END_INLINE_START")]
    BlockEndInlineStart,

    /// <summary>
    /// Equivalent to TOP_CENTER in both LTR and RTL.
    /// </summary>
    [EnumMember(Value = "BLOCK_START_INLINE_CENTER")]
    BlockStartInlineCenter,

    /// <summary>
    /// Equivalent to TOP_RIGHT in LTR, or TOP_LEFT in RTL.
    /// </summary>
    [EnumMember(Value = "BLOCK_START_INLINE_END")]
    BlockStartInlineEnd,

    /// <summary>
    /// Equivalent to TOP_LEFT in LTR, or TOP_RIGHT in RTL.
    /// </summary>
    [EnumMember(Value = "BLOCK_START_INLINE_START")]
    BlockStartInlineStart,

    /// <summary>
    /// Elements are positioned in the center of the bottom row.
    /// Consider using BLOCK_END_INLINE_CENTER instead.
    /// </summary>
    [EnumMember(Value = "BOTTOM_CENTER")]
    BottomCenter,

    /// <summary>
    /// Elements are positioned in the bottom left and flow towards the middle.
    /// Elements are positioned to the right of the Google logo.
    /// Consider using BLOCK_END_INLINE_START instead.
    /// </summary>
    [EnumMember(Value = "BOTTOM_LEFT")]
    BottomLeft,

    /// <summary>
    /// Elements are positioned in the bottom right and flow towards the middle.
    /// Elements are positioned to the left of the copyrights.
    /// Consider using BLOCK_END_INLINE_END instead.
    /// </summary>
    [EnumMember(Value = "BOTTOM_RIGHT")]
    BottomRight,

    /// <summary>
    /// Equivalent to RIGHT_CENTER in LTR, or LEFT_CENTER in RTL.
    /// </summary>
    [EnumMember(Value = "INLINE_END_BLOCK_CENTER")]
    InlineEndBlockCenter,

    /// <summary>
    /// Equivalent to RIGHT_BOTTOM in LTR, or LEFT_BOTTOM in RTL.
    /// </summary>
    [EnumMember(Value = "INLINE_END_BLOCK_END")]
    InlineEndBlockEnd,

    /// <summary>
    /// Equivalent to RIGHT_TOP in LTR, or LEFT_TOP in RTL.
    /// </summary>
    [EnumMember(Value = "INLINE_END_BLOCK_START")]
    InlineEndBlockStart,

    /// <summary>
    /// Equivalent to LEFT_CENTER in LTR, or RIGHT_CENTER in RTL.
    /// </summary>
    [EnumMember(Value = "INLINE_START_BLOCK_CENTER")]
    InlineStartBlockCenter,

    /// <summary>
    /// Equivalent to LEFT_BOTTOM in LTR, or RIGHT_BOTTOM in RTL.
    /// </summary>
    [EnumMember(Value = "INLINE_START_BLOCK_END")]
    InlineStartBlockEnd,

    /// <summary>
    /// Equivalent to LEFT_TOP in LTR, or RIGHT_TOP in RTL.
    /// </summary>
    [EnumMember(Value = "INLINE_START_BLOCK_START")]
    InlineStartBlockStart,

    /// <summary>
    /// Elements are positioned on the left, above bottom-left elements, and flow upwards.
    /// Consider using INLINE_START_BLOCK_END instead.
    /// </summary>
    [EnumMember(Value = "LEFT_BOTTOM")]
    LeftBottom,

    /// <summary>
    /// Elements are positioned in the center of the left side.
    /// Consider using INLINE_START_BLOCK_CENTER instead.
    /// </summary>
    [EnumMember(Value = "LEFT_CENTER")]
    LeftCenter,

    /// <summary>
    /// Elements are positioned on the left, below top-left elements, and flow downwards.
    /// Consider using INLINE_START_BLOCK_START instead.
    /// </summary>
    [EnumMember(Value = "LEFT_TOP")]
    LeftTop,

    /// <summary>
    /// Elements are positioned on the right, above bottom-right elements, and flow upwards.
    /// Consider using INLINE_END_BLOCK_END instead.
    /// </summary>
    [EnumMember(Value = "RIGHT_BOTTOM")]
    RightBottom,

    /// <summary>
    /// Elements are positioned in the center of the right side.
    /// Consider using INLINE_END_BLOCK_CENTER instead.
    /// </summary>
    [EnumMember(Value = "RIGHT_CENTER")]
    RightCenter,

    /// <summary>
    /// Elements are positioned on the right, below top-right elements, and flow downwards.
    /// Consider using INLINE_END_BLOCK_START instead.
    /// </summary>
    [EnumMember(Value = "RIGHT_TOP")]
    RightTop,

    /// <summary>
    /// Elements are positioned in the center of the top row.
    /// Consider using BLOCK_START_INLINE_CENTER instead.
    /// </summary>
    [EnumMember(Value = "TOP_CENTER")]
    TopCenter,

    /// <summary>
    /// Elements are positioned in the top left and flow towards the middle.
    /// Consider using BLOCK_START_INLINE_START instead.
    /// </summary>
    [EnumMember(Value = "TOP_LEFT")]
    TopLeft,

    /// <summary>
    /// Elements are positioned in the top right and flow towards the middle.
    /// Consider using BLOCK_START_INLINE_END instead.
    /// </summary>
    [EnumMember(Value = "TOP_RIGHT")]
    TopRight,
}
