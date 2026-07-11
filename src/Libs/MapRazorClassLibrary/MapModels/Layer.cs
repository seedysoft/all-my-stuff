namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public record Layer : Evented
{
    [J("options")] public override LayerOptions? Options { get; }

    public Layer(LayerOptions? layerOptions = default) : base(layerOptions) => Options = layerOptions;
}

public record LayerOptions : EventedOptions
{
    /// <summary>
    /// By default the layer will be added to the map's overlay pane.
    /// Overriding this option will cause the layer to be placed on another pane by default.
    /// Not effective if the renderer option is set (the renderer option will override the pane option).
    /// </summary>
    /// <remarks>Default: 'overlayPane'</remarks>
    [J("pane")] public string? Pane { get; set; }

    /// <summary>
    /// String to be shown in the attribution control, e.g. "© OpenStreetMap contributors".
    /// It describes the layer data and is often a legal obligation towards copyright holders and tile providers.
    /// </summary>
    /// <remarks>Default: null</remarks>
    [J("attribution")] public string? Attribution { get; set; }
}
