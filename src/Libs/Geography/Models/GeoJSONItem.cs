namespace Seedysoft.Libs.Geography.Models;

public class GeoJSONItem
{
    [J("type")] public string? Type { get; set; }

    [J("geometry")] public PointGeometry? Geometry { get; set; }
}

public class PointGeometry
{
    [J("type")] public string? Type { get; set; } = "Point";

    [J("coordinates")] public double[]? Coordinates { get; set; }

    [J("properties")] public Properties? Properties { get; set; }
}

/// <summary>
/// Properties class for custom attributes
/// </summary>
public class Properties
{
    /// <summary>
    /// 
    /// </summary>
    [J("name")] public string? Name { get; set; }
}

//public class GeoJsonObject
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [J("name")] public string? Name { get; set; }

//    /// <summary>
//    /// RFC 7946 array
//    /// </summary>
//    [J("data")] public Data[]? Data { get; set; }

//    /// <summary>
//    /// 
//    /// </summary>
//    [J("appearance")] public Appearance? Appearance { get; set; }

//    /// <summary>
//    /// 
//    /// </summary>
//    [J("tooltip")] public Tooltip? Tooltip { get; set; }
//}

//public class Data
//{
//    /// <summary>
//    /// "Feature", "FeatureCollection", or "GeometryCollection"
//    /// </summary>
//    [J("type")] public string? Type { get; set; }
//    /// <summary>
//    /// 
//    /// </summary>
//    [J("geometry")] public Geometry[]? Geometry { get; set; }
//}

///// <summary>
///// Geometry class for point features
///// </summary>
//public class Geometry
//{
//    /// <summary>
//    /// "Point", "MultiPoint", "LineString", "MultiLineString", "Polygon", "MultiPolygon", and "GeometryCollection"
//    /// </summary>
//    [J("type")] public string? Type { get; set; }

//    /// <summary>
//    /// 
//    /// </summary>

//    [J("coordinates")] public double[][]? Coordinates { get; set; }
//    //[J("coordinates")] public double[]? Coordinates { get; set; }

//    /// <summary>
//    /// 
//    /// </summary>
//    [J("properties")] public Properties? Properties { get; set; }
//}

///// <summary>
///// Appearance configuration controls the visual rendering of GeoJSON features. The custom extended format embeds appearance directly in the data file, eliminating post-load styling code.
///// </summary>
//public class Appearance : PolylineSymbol
//{
//    /// <summary>
//    /// Optional zoom-dependent tooltip visibility
//    /// </summary>
//    [J("visibilityZoomLevels")] public VisibilityZoomLevels? VisibilityZoomLevels { get; set; }
//}

///// <summary>
///// Tooltips provide interactive information display when users hover over or click map features. The custom GeoJSON format supports rich tooltip configuration including HTML content, template variables, and nested property display.
///// </summary>
//public class Tooltip
//{
//    /// <summary>
//    /// HTML content or template with variables
//    /// </summary>
//    [J("content")] public string? Content { get; set; }

//    /// <summary>
//    /// Tooltip opacity (0.0 to 1.0)
//    /// </summary>
//    [J("opacity")] public double Opacity { get; set; }

//    /// <summary>
//    /// Optional zoom-dependent tooltip visibility
//    /// </summary>
//    [J("visibilityZoomLevels")] public VisibilityZoomLevels? VisibilityZoomLevels { get; set; }
//}

///// <summary>
///// Features are only visible when the map zoom level is between <see cref="MinZoomLevel"/> and <see cref="MaxZoomLevel"/> (inclusive).
///// </summary>
//public class VisibilityZoomLevels
//{
//    /// <summary>
//    /// Minimum zoom level at which the tooltip or icon becomes visible. Below this level, the element will be hidden to reduce clutter on the map. This allows for a cleaner view at wider zoom levels while still providing detailed information as users zoom in.
//    /// </summary>
//    [J("minZoomLevel")] public int? MinZoomLevel { get; set; }

//    /// <summary>
//    /// Maximum zoom level at which the tooltip or icon remains visible. Above this level, the element will be hidden to prevent overcrowding of information at very close zoom levels. This helps maintain a clear and user-friendly map interface by controlling the visibility of features based on the zoom level.
//    /// </summary>
//    [J("maxZoomLevel")] public int? MaxZoomLevel { get; set; }
//}

//public class PointIcon
//{
//    /// <summary>
//    /// URL of the icon image
//    /// </summary>
//    [J("iconUrl")] public string? IconUrl { get; set; }

//    /// <summary>
//    /// Icon dimensions [width, height] in pixels
//    /// </summary>
//    [J("iconSize")] public int[]? IconSize { get; set; }

//    /// <summary>
//    /// Anchor point of the icon in pixels [x, y] from the top-left corner
//    /// </summary>
//    [J("iconAnchor")] public int[]? IconAnchor { get; set; }

//    /// <summary>
//    /// Popup anchor offset [x, y]
//    /// </summary>
//    [J("popupAnchor")] public int[]? PopupAnchor { get; set; }
//}

//public class PolygonSymbol : PolylineSymbol
//{
//    /// <summary>
//    /// Fill color
//    /// </summary>
//    [J("fillColor")] public string? FillColor { get; set; }

//    /// <summary>
//    /// Fill opacity
//    /// </summary>
//    [J("fillOpacity")] public double FillOpacity { get; set; }

//    ///// <summary>
//    ///// Stroke color
//    ///// </summary>
//    // [J("color")] public string? Color { get; set; }

//    ///// <summary>
//    ///// Stroke opacity
//    ///// </summary>
//    // [J("opacity")] public double Opacity { get; set; }

//    ///// <summary>
//    ///// Stroke width in pixels
//    ///// </summary>
//    // [J("weight")] public int Weight { get; set; }
//}

///// <summary>
///// The PointSymbol class defines circular marker symbols for StreamPoint visualization. It provides properties for controlling both the marker's outline (stroke) and interior (fill).
///// </summary>
//public class PointSymbol : PolylineSymbol
//{
//    /// <summary>
//    /// Circle radius in pixels
//    /// </summary>
//    [J("radius")] public int Radius { get; set; }

//    /// <summary>
//    /// Fill color
//    /// </summary>
//    [J("fillColor")] public string? FillColor { get; set; }

//    /// <summary>
//    /// Fill opacity
//    /// </summary>
//    [J("fillOpacity")] public double FillOpacity { get; set; }

//    ///// <summary>
//    ///// Stroke color
//    ///// </summary>
//    // [J("color")] public string? Color { get; set; }

//    ///// <summary>
//    ///// Stroke opacity
//    ///// </summary>
//    // [J("opacity")] public double Opacity { get; set; }

//    ///// <summary>
//    ///// Stroke width in pixels
//    ///// </summary>
//    // [J("weight")] public int Weight { get; set; }
//}

//public class PolylineSymbol
//{
//    /// <summary>
//    /// Line color
//    /// </summary>
//    [J("color")] public string? Color { get; set; }

//    /// <summary>
//    /// Line opacity
//    /// </summary>
//    [J("opacity")] public double Opacity { get; set; }

//    /// <summary>
//    /// Line width in pixels
//    /// </summary>
//    [J("weight")] public int Weight { get; set; }
//}
