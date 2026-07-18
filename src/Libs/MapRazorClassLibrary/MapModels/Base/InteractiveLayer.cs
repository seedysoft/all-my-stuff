namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Base;

/// <summary>
/// Some <see cref="Layer"/>s can be made interactive - when the user interacts with such a layer, pointer events like <c>click</c> and <c>pointerover</c> can be handled.
/// Use the <see href="https://leafletjs.com/reference-2.0.0.html#evented-method">event handling methods</see> to handle these events.
/// </summary>
public abstract record class InteractiveLayer : Layer
{
    public InteractiveLayer() : base() { }

    /// <summary>
    /// If false, the layer will not emit pointer events and will act as a part of the underlying map.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("interactive")] public bool? Interactive { get; set; } = true;

    /// <summary>
    /// When true, a pointer event on this layer will trigger the same event on the map (unless <see href="https://leafletjs.com/reference-2.0.0.html#domevent-stoppropagation">DomEvent.stopPropagation</see> is used).
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("bubblingPointerEvents")] public bool? BubblingPointerEvents { get; set; } = true;
}
