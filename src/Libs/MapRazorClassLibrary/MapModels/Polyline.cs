namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// A class for drawing polyline overlays on a map.
/// </summary>
public sealed record Polyline : Path
{
    /// <summary>
    /// Array of geographical points
    /// </summary>
    [J("points")] public IReadOnlyList<LatLng> Points { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [J("options")] public override PolylineOptions? Options { get; }

    public Polyline(IReadOnlyList<LatLng> points, PolylineOptions? polylineOptions = default) : base(polylineOptions)
    {
        Points = points;
        Options = polylineOptions;
    }
}

public record PolylineOptions : PathOptions
{
    /// <summary>
    /// How much to simplify the polyline on each zoom level.
    /// More means better performance and smoother look, and less means more accurate representation.
    /// </summary>
    /// <remarks>Default: 1.0</remarks>
    [J("smoothFactor")] public double? SmoothFactor { get; set; }
}
