namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Base;

/// <summary>
/// Base class for vector renderer implementations (<see cref="SVG"/>, <see cref="Canvas"/>).
/// Handles the DOM container of the renderer, its bounds, and its zoom animation.
/// A <see cref="Renderer"/> works as an implicit layer group for all <see cref="Path"/>s - the renderer itself can be added or removed to the map.
/// All paths use a renderer, which can be implicit (the map will decide the type of renderer and use it automatically) or explicit (using the renderer option of the path).
/// Do not use this class directly, use SVG and Canvas instead.
/// The <see cref="BlanketOverlay.Continuous"/> option inherited from <see cref="BlanketOverlay"/> cannot be set to <c>true</c> (otherwise, renderers get out of place during a pinch-zoom operation).
/// </summary>
public abstract record class Renderer : BlanketOverlay
{
    public Renderer() : base() { }
}
