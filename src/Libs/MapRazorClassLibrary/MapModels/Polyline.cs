namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// A class for drawing polyline overlays on a map.
/// </summary>
public record class Polyline : Path
{
    public Polyline(IReadOnlyList<LatLng> points) => Points = points;

    /// <summary>
    /// Array of geographical points
    /// </summary>
    [J("points")] public IReadOnlyList<LatLng> Points { get; set; }

    /// <summary>
    /// How much to simplify the polyline on each zoom level.
    /// More means better performance and smoother look, and less means more accurate representation.
    /// </summary>
    /// <remarks>Default: 1.0</remarks>
    [J("smoothFactor")] public double? SmoothFactor { get; set; }
}
