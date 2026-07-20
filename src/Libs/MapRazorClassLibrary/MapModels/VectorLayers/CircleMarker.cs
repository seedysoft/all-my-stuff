namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.VectorLayers;

/// <summary>
/// A circle of a fixed size with radius specified in pixels.
/// </summary>
public sealed record class CircleMarker : Path
{
    public CircleMarker(Basic.LatLng position) : base()
    {
        Position = position;
        Pane = Map.Panes.OverlayPane;
    }

    /// <summary>
    /// Geographical point.
    /// </summary>
    [J("position")] public Basic.LatLng Position { get; }

    /// <summary>
    /// Radius of the circle marker, in pixels.
    /// </summary>
    /// <remarks>Default: <c>10</c></remarks>
    [J("radius")] public double? Radius { get; set; } = 10;
}
