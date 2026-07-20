namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Base;

/// <summary>
/// Represents an HTML element that covers ("blankets") the entire surface of the map.
/// Do not use this class directly.
/// It's meant for <see cref="Renderer"/>, and for plugins that rely on one single HTML element.
/// </summary>
public abstract record class BlanketOverlay : Layer
{
    public BlanketOverlay() : base() { }

    /// <summary>
    /// How much to extend the clip area around the map view (relative to its size) e.g. 0.1 would be 10% of map view in each direction.
    /// </summary>
    /// <remarks>Default: <c>0.1</c></remarks>
    [J("padding")] public double? Padding { get; set; } = 0.1;

    /// <summary>
    /// When <c>false</c>, the blanket will update its position only when the map state settles (after a pan/zoom animation).
    /// When <c>true</c>, it will update when the map state changes (during pan/zoom animations).
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("continuous")] public bool? Continuous { get; set; } = false;
}
