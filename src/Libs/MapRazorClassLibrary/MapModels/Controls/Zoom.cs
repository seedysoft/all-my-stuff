namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Controls;

/// <summary>
/// A basic zoom control with two buttons (zoom in and zoom out).
/// It is put on the map by default unless you set its <see cref="Map.ZoomControl"/> option to <c>false</c>.
/// Extends <see cref="Base.Control"/>.
/// </summary>
public sealed record class Zoom : Base.Control
{
    public Zoom() : base() => Position = Positions.TopLeft;

    /// <summary>
    /// The text set on the 'zoom in' button.
    /// </summary>
    /// <remarks>Default: </remarks>
    [J("zoomInText")] public string? ZoomInText { get; set; } = "<span aria-hidden='true'>+</span>";
}
