namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Base;

/// <summary>
/// Control is a base class for implementing map controls. Handles positioning.
/// All other controls extend from this class.
/// </summary>
public abstract record class Control
{
    /// <summary>
    /// The position of the control (one of the map corners).
    /// Possible values are <c>ControlPosition.TopLeft</c>, <c>ControlPosition.TopRight</c>, <c>ControlPosition.BottomLeft</c> or <c>ControlPosition.BottomRight</c>
    /// </summary>
    /// <remarks>Default: <c>ControlPosition.TopRight</c></remarks>
    [J("position")] public Positions Position { get; set; } = Positions.TopRight;

    public enum Positions
    {
        [J("topleft")] TopLeft,
        [J("topright")] TopRight,
        [J("bottomleft")] BottomLeft,
        [J("bottomright")] BottomRight
    }
}
