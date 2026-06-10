namespace Seedysoft.Libs.GasStationPrices.Models;

public class GeoJSONPointAppearanceClass
{
    [J("data")] public GeoJSONItem[] Data { get; set; } = [];
    [J("name")] public string? Name { get; set; } = string.Empty;
    [J("symbology")] public PointSymbol? Symbology { get; set; }
    [J("tooltip")] public Tooltip? Tooltip { get; set; }
}

public class Tooltip
{
    [J("content")] public string? Content { get; set; }//js string template
    [J("offset")] public int[]? Offset { get; set; }
    [J("permanent")] public bool Permanent { get; set; }
    [J("opacity")] public double Opacity { get; set; }
    [J("coordinateInversion")] public bool CoordinateInversion { get; set; }
    [J("visibilityZoomLevels")] public VisibilityZoomLevel? VisibilityZoomLevels { get; set; }
}

public class VisibilityZoomLevel
{
    [J("minZoomLevel")] public int? MinZoomLevel { get; set; } = 0;
    [J("maxZoomLevel")] public int? MaxZoomLevel { get; set; } = 0;
}

public class GeoJSONPolygonAppearanceClass
{
    [J("data")] public GeoJSONLineStringClass[] Data { get; set; } = [];
    [J("name")] public string? Name { get; set; } = string.Empty;
    [J("symbology")] public PolygonSymbol? Symbology { get; set; }
    [J("tooltip")] public Tooltip? Tooltip { get; set; }
}

public class PointSymbol : PolygonSymbol
{
    [J("radius")] public int Radius { get; set; } = 4;
    [J("fillColor")] public string? FillColor { get; set; }
    [J("fillOpacity")] public double FillOpacity { get; set; } = 1;
}

public class PolygonSymbol
{
    [J("color")] public string? Color { get; set; }
    [J("opacity")] public double Opacity { get; set; }
    [J("weight")] public int Weight { get; set; }
}

public class GeoJSONItem
{
    [J("type")] public string? Type { get; set; } = "Feature";
    [J("geometry")] public PointGeometry? Geometry { get; set; }
    [J("properties")] public Properties? Properties { get; set; }
}

public class GeoJSONLineStringClass
{
    [J("type")] public string? Type { get; set; } = "Feature";
    [J("geometry")] public LineStringGeometry? Geometry { get; set; }
    [J("properties")] public Properties? Properties { get; set; }
}

public class PointGeometry
{
    [J("type")] public string? Type { get; set; } = "Point";
    /// <summary>
    /// Coordinates are in the format: [lat, lng]
    /// </summary>
    [J("coordinates")] public double[]? Coordinates { get; set; }
}

public class LineStringGeometry
{
    [J("type")] public string? Type { get; set; } = "LineString";
    /// <summary>
    /// Coordinates are in the format: [lat, lng]
    /// </summary>
    [J("coordinates")] public double[][]? Coordinates { get; set; }
}

public class Properties
{
    [J("name")] public string? Name { get; set; }
}
