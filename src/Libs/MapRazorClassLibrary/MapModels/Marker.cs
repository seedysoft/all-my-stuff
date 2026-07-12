using OneOf;

namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// Marker is used to display clickable/draggable icons on the map.
/// </summary>
public record Marker : InteractiveLayer
{
    /// <summary>
    /// Geographical point.
    /// </summary>
    [J("position")] public LatLng Position { get; }

    [J("options")] public override MarkerOptions? Options { get; }

    public Marker(LatLng position, MarkerOptions? markerOptions = default) : base(markerOptions)
    {
        Position = position;
        Options = markerOptions;
    }
}

public record MarkerOptions : InteractiveLayerOptions
{
    /// <summary>
    /// Text for the alt attribute of the icon image.
    /// <see href="https://leafletjs.com/examples/accessibility/#markers-must-be-labelled">Useful for accessibility.</see>
    /// </summary>
    /// <remarks>Default: 'Marker'</remarks>
    [J("alt")] public string? Alt { get; set; }

    /// <summary>
    /// When true, the map will pan whenever the marker is focused (via e.g. pressing tab on the keyboard) to ensure the marker is visible within the map's bounds.
    /// </summary>
    /// <remarks>Default: <code>true</code></remarks>
    [J("autoPanOnFocus")] public bool? AutoPanOnFocus { get; set; }

    /// <summary>
    /// Icon instance to use for rendering the marker.
    /// See <see href="https://leafletjs.com/reference-2.0.0.html#icon">Icon documentation</see> for details on how to customize the marker icon.
    /// If not specified, a common instance of <see cref="Icon.Default"/> is used.
    /// </summary>
    [J("icon")] public Icon? Icon { get; set; }

    /// <summary>
    /// Whether the marker can be tabbed to with a keyboard and clicked by pressing enter.
    /// </summary>
    /// <remarks>Default: <code>true</code></remarks>
    [J("keyboard")] public bool? Keyboard { get; set; }

    /// <summary>
    /// The opacity of the marker.
    /// </summary>
    /// <remarks>Default: 1.0</remarks>
    [J("opacity")] public double? Opacity { get; set; }

    /// <summary>
    /// Map pane where the markers icon will be added.
    /// </summary>
    /// <remarks>Default: 'markerPane'</remarks>
    [J("pane")] public new string? Pane { get; set; }

    /// <summary>
    /// The z-index offset used for the riseOnHover feature.
    /// </summary>
    /// <remarks>Default: 250</remarks>
    [J("riseOffset")] public double? RiseOffset { get; set; }

    /// <summary>
    /// If true, the marker will get on top of others when you hover the pointer over it.
    /// </summary>
    /// <remarks>Default: <code>false</code></remarks>
    [J("riseOnHover")] public bool? RiseOnHover { get; set; }

    /// <summary>
    /// Map pane where the markers shadow will be added.
    /// </summary>
    /// <remarks>Default: 'shadowPane'</remarks>
    [J("shadowPane")] public string? ShadowPane { get; set; }

    /// <summary>
    /// Text for the browser tooltip that appear on marker hover (no tooltip by default).
    /// <see href="https://leafletjs.com/examples/accessibility/#markers-must-be-labelled">Useful for accessibility.</see>
    /// </summary>
    /// <remarks>Default: ''</remarks>
    [J("title")] public string? Title { get; set; }

    /// <summary>
    /// By default, marker images zIndex is set automatically based on its latitude.
    /// Use this option if you want to put the marker on top of all others (or below), specifying a high value like 1000 (or high negative value, respectively).
    /// </summary>
    /// <remarks>Default: 0</remarks>
    [J("zIndexOffset")] public double? ZIndexOffset { get; set; }
}

/// <summary>
/// Represents an icon to provide when creating a marker.
/// </summary>
public sealed class Icon
{
    /// <summary>
    /// The URL to the icon image (absolute or relative to your script path).
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("iconUrl")] public required string IconUrl { get; set; }

    /// <summary>
    /// The URL to a retina sized version of the icon image (absolute or relative to your script path).
    /// Used for Retina screen devices.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("iconRetinaUrl")] public string? IconRetinaUrl { get; set; }

    /// <summary>
    /// Size of the icon image in pixels.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("iconSize")] public Point? IconSize { get; set; }

    /// <summary>
    /// The coordinates of the "tip" of the icon (relative to its top left corner).
    /// The icon will be aligned so that this point is at the marker's geographical location.
    /// Centered by default if size is specified, also can be set in CSS with negative margins.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("iconAnchor")] public Point? IconAnchor { get; set; }

    /// <summary>
    /// The coordinates of the point from which popups will "open", relative to the icon anchor.
    /// </summary>
    /// <remarks>Default: [0, 0]</remarks>
    [J("popupAnchor")] public Point PopupAnchor { get; set; } = new Point(0, 0);

    /// <summary>
    /// The coordinates of the point from which tooltips will "open", relative to the icon anchor.
    /// </summary>
    /// <remarks>Default: [0, 0]</remarks>
    [J("tooltipAnchor")] public Point TooltipAnchor { get; set; } = new Point(0, 0);

    /// <summary>
    /// The URL to the icon shadow image.
    /// If not specified, no shadow image will be created.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("shadowUrl")] public string? ShadowUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("shadowRetinaUrl")] public string? ShadowRetinaUrl { get; set; }

    /// <summary>
    /// Size of the shadow image in pixels.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("shadowSize")] public Point? ShadowSize { get; set; }

    /// <summary>
    /// The coordinates of the "tip" of the shadow (relative to its top left corner) (the same as iconAnchor if not specified).
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("shadowAnchor")] public Point? ShadowAnchor { get; set; }

    /// <summary>
    /// A custom class name to assign to both icon and shadow images.
    /// Empty by default.
    /// </summary>
    /// <remarks>Default: ''</remarks>
    [J("className")] public string? ClassName { get; set; } = string.Empty;

    ///// <summary>
    ///// Whether the crossOrigin attribute will be added to the tiles.
    ///// If a String is provided, all tiles will have their crossOrigin attribute set to the String provided.
    ///// This is needed if you want to access tile pixel data.
    ///// Refer to CORS Settings for valid String values.
    ///// </summary>
    ///// <remarks>Default: <code>false</code></remarks>
    //[J("crossOrigin")] public OneOf<bool, string> CrossOrigin { get; set; }

    //private static readonly IconOptions DefaultIconOptions = new()
    //{
    //    IconUrl = "marker-icon.png",
    //    IconRetinaUrl = "marker-icon-2x.png",
    //    ShadowUrl = "marker-shadow.png",
    //    IconSize = new Point(25, 41),
    //    IconAnchor = new Point(12, 41),
    //    PopupAnchor = new Point(1, -34),
    //    TooltipAnchor = new Point(16, -28),
    //    ShadowSize = new Point(41, 41),
    //};

    /// <summary>
    /// A trivial subclass of <see cref="Icon"/>, represents the icon to use in Markers when no icon is specified.
    /// Points to the blue marker image distributed with Leaflet releases.
    /// </summary>
    public static Icon Default { get; } = new()
    {
        IconUrl = "marker-icon.png",
        IconRetinaUrl = "marker-icon-2x.png",
        ShadowUrl = "marker-shadow.png",
        IconSize = new Point(25, 41),
        IconAnchor = new Point(12, 41),
        PopupAnchor = new Point(1, -34),
        TooltipAnchor = new Point(16, -28),
        ShadowSize = new Point(41, 41),
    };

    //[J("options")] public IconOptions? Options { get; }

    private Icon() { }
    //private Icon(IconOptions? iconOptions = default) => Options = iconOptions;

    internal static Icon Create(
        string? iconUrl = default,
        string? iconRetinaUrl = default,
        Point? iconSize = default,
        Point? iconAnchor = default,
        Point? popupAnchor = default,
        Point? tooltipAnchor = default,
        string? shadowUrl = default,
        string? shadowRetinaUrl = default,
        Point? shadowSize = default,
        Point? shadowAnchor = default,
        string? className = default
        //, OneOf<bool, string>? crossOrigin = default
        )
    {
        //IconOptions options = new()
        //{
        //    IconUrl = iconUrl ?? DefaultIconOptions.IconUrl,
        //    IconRetinaUrl = iconRetinaUrl ?? DefaultIconOptions.IconRetinaUrl,
        //    IconSize = iconSize ?? DefaultIconOptions.IconSize,
        //    IconAnchor = iconAnchor ?? DefaultIconOptions.IconAnchor,
        //    PopupAnchor = popupAnchor ?? DefaultIconOptions.PopupAnchor,
        //    TooltipAnchor = tooltipAnchor ?? DefaultIconOptions.TooltipAnchor,
        //    ShadowUrl = shadowUrl ?? DefaultIconOptions.ShadowUrl,
        //    ShadowRetinaUrl = shadowRetinaUrl ?? DefaultIconOptions.ShadowRetinaUrl,
        //    ShadowSize = shadowSize ?? DefaultIconOptions.ShadowSize,
        //    ShadowAnchor = shadowAnchor ?? DefaultIconOptions.ShadowAnchor,
        //    ClassName = className ?? DefaultIconOptions.ClassName,
        //    CrossOrigin = crossOrigin ?? DefaultIconOptions.CrossOrigin,
        //};

        // Create a new Icon instance using the parameterless constructor.
        // This keeps behavior consistent with callers that expect a distinct Icon object
        // while allowing null to represent the Default icon (callers use null for Default).
        return new Icon()
        {
            IconUrl = iconUrl ?? Default.IconUrl,
            IconRetinaUrl = iconRetinaUrl ?? Default.IconRetinaUrl,
            IconSize = iconSize ?? Default.IconSize,
            IconAnchor = iconAnchor ?? Default.IconAnchor,
            PopupAnchor = popupAnchor ?? Default.PopupAnchor,
            TooltipAnchor = tooltipAnchor ?? Default.TooltipAnchor,
            ShadowUrl = shadowUrl ?? Default.ShadowUrl,
            ShadowRetinaUrl = shadowRetinaUrl ?? Default.ShadowRetinaUrl,
            ShadowSize = shadowSize ?? Default.ShadowSize,
            ShadowAnchor = shadowAnchor ?? Default.ShadowAnchor,
            ClassName = className ?? Default.ClassName,
            //CrossOrigin = crossOrigin ?? Default.CrossOrigin,
        };
        ;
    }
}

//public abstract class IconOptions
//{
//    /// <summary>
//    /// The URL to the icon image (absolute or relative to your script path).
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("iconUrl")] public required string IconUrl { get; set; }

//    /// <summary>
//    /// The URL to a retina sized version of the icon image (absolute or relative to your script path).
//    /// Used for Retina screen devices.
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("iconRetinaUrl")] public string? IconRetinaUrl { get; set; }

//    /// <summary>
//    /// Size of the icon image in pixels.
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("iconSize")] public Point? IconSize { get; set; }

//    /// <summary>
//    /// The coordinates of the "tip" of the icon (relative to its top left corner).
//    /// The icon will be aligned so that this point is at the marker's geographical location.
//    /// Centered by default if size is specified, also can be set in CSS with negative margins.
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("iconAnchor")] public Point? IconAnchor { get; set; }

//    /// <summary>
//    /// The coordinates of the point from which popups will "open", relative to the icon anchor.
//    /// </summary>
//    /// <remarks>Default: [0, 0]</remarks>
//    [J("popupAnchor")] public Point PopupAnchor { get; set; } = new Point(0, 0);

//    /// <summary>
//    /// The coordinates of the point from which tooltips will "open", relative to the icon anchor.
//    /// </summary>
//    /// <remarks>Default: [0, 0]</remarks>
//    [J("tooltipAnchor")] public Point TooltipAnchor { get; set; } = new Point(0, 0);

//    /// <summary>
//    /// The URL to the icon shadow image.
//    /// If not specified, no shadow image will be created.
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("shadowUrl")] public string? ShadowUrl { get; set; }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("shadowRetinaUrl")] public string? ShadowRetinaUrl { get; set; }

//    /// <summary>
//    /// Size of the shadow image in pixels.
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("shadowSize")] public Point? ShadowSize { get; set; }

//    /// <summary>
//    /// The coordinates of the "tip" of the shadow (relative to its top left corner) (the same as iconAnchor if not specified).
//    /// </summary>
//    /// <remarks>Default: null</remarks>
//    [J("shadowAnchor")] public Point? ShadowAnchor { get; set; }

//    /// <summary>
//    /// A custom class name to assign to both icon and shadow images.
//    /// Empty by default.
//    /// </summary>
//    /// <remarks>Default: ''</remarks>
//    [J("className")] public string? ClassName { get; set; } = string.Empty;

//    /// <summary>
//    /// Whether the crossOrigin attribute will be added to the tiles.
//    /// If a String is provided, all tiles will have their crossOrigin attribute set to the String provided.
//    /// This is needed if you want to access tile pixel data.
//    /// Refer to CORS Settings for valid String values.
//    /// </summary>
//    /// <remarks>Default: <code>false</code></remarks>
//    [J("crossOrigin")] public OneOf<bool, string> CrossOrigin { get; set; }
//}

/// <summary>
/// Represents a point with x and y coordinates in pixels.
/// </summary>
public readonly record struct Point
{
    [J("x")] public double X { get; init; }
    [J("y")] public double Y { get; init; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}
