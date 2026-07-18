// TODO                                 perform icon images

namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Basic;

/// <summary>
/// Represents an icon to provide when creating a marker.
/// </summary>
public sealed record class Icon
{
    public Icon(string iconUrl = "marker-icon.png")
    {
        IconUrl = iconUrl;
        IconRetinaUrl = Default?.IconRetinaUrl;
        IconSize = Default?.IconSize;
        IconAnchor = Default?.IconAnchor;
        PopupAnchor = Default?.PopupAnchor;
        TooltipAnchor = Default?.TooltipAnchor;
        ShadowUrl = Default?.ShadowUrl;
        ShadowRetinaUrl = Default?.ShadowRetinaUrl;
        ShadowSize = Default?.ShadowSize;
        ShadowAnchor = Default?.ShadowAnchor;
        ClassName = Default?.ClassName;
        //CrossOrigin = Default?.CrossOrigin;
    }

    /// <summary>
    /// The URL to the icon image (absolute or relative to your script path).
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("iconUrl")] public required string IconUrl { get; set; }

    /// <summary>
    /// The URL to a retina sized version of the icon image (absolute or relative to your script path).
    /// Used for Retina screen devices.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("iconRetinaUrl")] public string? IconRetinaUrl { get; set; }

    /// <summary>
    /// Size of the icon image in pixels.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("iconSize")] public Point? IconSize { get; set; }

    /// <summary>
    /// The coordinates of the "tip" of the icon (relative to its top left corner).
    /// The icon will be aligned so that this point is at the marker's geographical location.
    /// Centered by default if size is specified, also can be set in CSS with negative margins.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("iconAnchor")] public Point? IconAnchor { get; set; }

    /// <summary>
    /// The coordinates of the point from which popups will "open", relative to the icon anchor.
    /// </summary>
    /// <remarks>Default: [0, 0]</remarks>
    [J("popupAnchor")] public Point? PopupAnchor { get; set; } = new Point(0, 0);

    /// <summary>
    /// The coordinates of the point from which tooltips will "open", relative to the icon anchor.
    /// </summary>
    /// <remarks>Default: [0, 0]</remarks>
    [J("tooltipAnchor")] public Point? TooltipAnchor { get; set; } = new Point(0, 0);

    /// <summary>
    /// The URL to the icon shadow image.
    /// If not specified, no shadow image will be created.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("shadowUrl")] public string? ShadowUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("shadowRetinaUrl")] public string? ShadowRetinaUrl { get; set; }

    /// <summary>
    /// Size of the shadow image in pixels.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("shadowSize")] public Point? ShadowSize { get; set; }

    /// <summary>
    /// The coordinates of the "tip" of the shadow (relative to its top left corner) (the same as iconAnchor if not specified).
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("shadowAnchor")] public Point? ShadowAnchor { get; set; }

    /// <summary>
    /// A custom class name to assign to both icon and shadow images.
    /// Empty by default.
    /// </summary>
    /// <remarks>Default: ''</remarks>
    [J("className")] public string? ClassName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the crossOrigin attribute will be added to the tiles.
    /// If a String is provided, all tiles will have their crossOrigin attribute set to the String provided.
    /// This is needed if you want to access tile pixel data.
    /// Refer to CORS Settings for valid String values.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("crossOrigin"), K(typeof(OneOf.Serialization.SystemTextJson.OneOfJsonConverter))]
    public OneOf.OneOf<bool, string>? CrossOrigin { get; set; } = false;

    /// <summary>
    /// A trivial subclass of <see cref="Icon"/>, represents the icon to use in Markers when no icon is specified.
    /// Points to the blue marker image distributed with Leaflet releases.
    /// </summary>
    public static Icon Default { get; } = new()
    {
        IconUrl = "marker-icon.png",
        //IconRetinaUrl = "marker-icon-2x.png",
        //ShadowUrl = "marker-shadow.png",
        IconSize = new Point(22, 22),
        //IconAnchor = new Point(12, 41),
        //PopupAnchor = new Point(1, -34),
        //TooltipAnchor = new Point(16, -28),
        //ShadowSize = new Point(41, 41),
        //CrossOrigin = false,
    };
}
