namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Controls;

/// <summary>
/// A simple scale control that shows the scale of the current center of screen in metric (m/km) and imperial (mi/ft) systems.
/// Extends <see cref="Base.Control"/>.
/// </summary>
internal sealed class Scale : Base.Control
{
    public Scale() : base() => Position = Positions.BottomLeft;

    /// <summary>
    /// Maximum width of the control in pixels.
    /// The width is set dynamically to show round values (e.g. 100, 200, 500).
    /// </summary>
    /// <remarks>Default: <c>100</c></remarks>
    [J("maxWidth")] public double? MaxWidth { get; set; } = 100;

    //    /// <summary>
    //    /// Whether to show the imperial scale line (mi/ft).
    //    /// </summary>
    //    /// <remarks>Default is <see langword="true"/></remarks>
    //    public bool? Imperial { get; set; } = true;

    //    /// <summary>
    //    /// Whether to show the metric scale line (m/km).
    //    /// </summary>
    //    /// <remarks>Default is <see langword="true"/></remarks>
    //    public bool? Metric { get; set; }

    //    /// <summary>
    //    /// If true, the control is updated on moveend, otherwise it's always up-to-date (updated on move).
    //    /// </summary>
    //    /// <remarks>Default is <see langword="false"/></remarks>
    //    public bool? UpdateWhenIdle { get; set; }
}
