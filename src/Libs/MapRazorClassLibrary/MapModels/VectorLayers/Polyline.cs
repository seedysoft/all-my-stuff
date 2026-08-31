namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.VectorLayers;

/// <summary>
/// A class for drawing polyline overlays on a map.
/// Extends <see cref="Path"/>.
/// </summary>
internal sealed class Polyline(IReadOnlyList<Basic.LatLng> points) : Path()
{

    /// <summary>
    /// Array of geographical points
    /// </summary>
    [J("points")] public IReadOnlyList<Basic.LatLng> Points { get; set; } = points;

    /// <summary>
    /// How much to simplify the polyline on each zoom level.
    /// More means better performance and smoother look, and less means more accurate representation.
    /// </summary>
    /// <remarks>Default: <c>1.0</c></remarks>
    [J("smoothFactor")] public double? SmoothFactor { get; set; } = 1.0;

    /// <summary>
    /// Disable polyline clipping.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("noClip")] public bool? NoClip { get; set; } = false;
}
