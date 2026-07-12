namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public record InteractiveLayer : Layer
{
    [J("options")] public override InteractiveLayerOptions? Options { get; }

    public InteractiveLayer(InteractiveLayerOptions? layerOptions = default) : base(layerOptions) => Options = layerOptions;
}

public record InteractiveLayerOptions : LayerOptions
{
    /// <summary>
    /// If false, the layer will not emit pointer events and will act as a part of the underlying map.
    /// </summary>
    /// <remarks>Default: <code>true</code></remarks>
    [J("interactive")] public bool? Interactive { get; set; }

    /// <summary>
    /// When true, a pointer event on this layer will trigger the same event on the map (unless <see href="https://leafletjs.com/reference-2.0.0.html#domevent-stoppropagation">DomEvent.stopPropagation</see> is used).
    /// </summary>
    /// <remarks>Default: <code>true</code></remarks>
    [J("bubblingPointerEvents")] public bool? BubblingPointerEvents { get; set; }
}
