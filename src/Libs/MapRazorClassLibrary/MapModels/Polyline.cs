namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// A class for drawing polyline overlays on a map.
/// Extends <see cref="Path"/>.
/// </summary>
public record class Polyline : Path
{
    public Polyline(IReadOnlyList<LatLng> points) : base() => Points = points;

    /// <summary>
    /// Array of geographical points
    /// </summary>
    [J("points")] public IReadOnlyList<LatLng> Points { get; set; }

    /// <summary>
    /// How much to simplify the polyline on each zoom level.
    /// More means better performance and smoother look, and less means more accurate representation.
    /// </summary>
    /// <remarks>Default: 1.0</remarks>
    [J("smoothFactor")] public double? SmoothFactor { get; set; } = 1.0;

    /// <summary>
    /// Disable polyline clipping.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("noClip")] public bool? NoClip { get; set; } = false;
}
