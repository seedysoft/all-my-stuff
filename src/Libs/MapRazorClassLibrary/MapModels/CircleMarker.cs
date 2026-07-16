namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// A circle of a fixed size with radius specified in pixels.
/// </summary>
public record class CircleMarker : Path
{
    public CircleMarker(LatLng position) => Position = position;

    /// <summary>
    /// Geographical point.
    /// </summary>
    [J("position")] public LatLng Position { get; }

    /// <summary>
    /// Map pane where the circle markers will be added.
    /// </summary>
    /// <remarks>Default: 'overlayPane'</remarks>
    [J("pane")] public new string? Pane { get; set; } = "overlayPane";

    /// <summary>
    /// Radius of the circle marker, in pixels.
    /// </summary>
    /// <remarks>Default: 10</remarks>
    [J("radius")] public double? Radius { get; set; } = 10;
}
