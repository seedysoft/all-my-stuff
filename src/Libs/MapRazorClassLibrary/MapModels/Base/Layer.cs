namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Base;

/// <summary>
/// A set of methods from the Layer base class that all Leaflet layers use.
/// Inherits all methods, options and events from <see cref="Evented"/>.
/// </summary>
internal abstract class Layer : Evented
{
    public Layer() : base() { }

    /// <summary>
    /// By default the layer will be added to the map's overlay pane.
    /// Overriding this option will cause the layer to be placed on another pane by default.
    /// Not effective if the renderer option is set (the renderer option will override the pane option).
    /// </summary>
    /// <remarks>Default: <c>'overlayPane'</c></remarks>
    [J("pane")] public Map.Panes? Pane { get; set; } = Map.Panes.OverlayPane;

    /// <summary>
    /// String to be shown in the attribution control, e.g. "© OpenStreetMap contributors".
    /// It describes the layer data and is often a legal obligation towards copyright holders and tile providers.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("attribution")] public string? Attribution { get; set; } = null;
}
