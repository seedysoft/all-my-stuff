namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.UILayers;

/// <summary>
/// Base model for <see cref="Popup"/> and <see cref="Tooltip"/>.
/// Inherit from it for custom overlays like plugins.
/// </summary>
public abstract record class DivOverlay : Base.InteractiveLayer
{
    protected DivOverlay() : base()
    {
        Interactive = false;
        Pane = null;
    }

    /// <summary>
    /// The offset of the overlay position.
    /// </summary>
    /// <remarks>Default: <c>Point(0, 0)</c></remarks>
    [J("offset")] public Basic.Point? Offset { get; set; } = Basic.Point.Empty;

    /// <summary>
    /// A custom CSS class name to assign to the overlay.
    /// </summary>
    /// <remarks>Default: <c>''</c></remarks>
    [J("className")] public string? ClassName { get; set; } = string.Empty;

    /// <summary>
    /// Sets the HTML content of the overlay while initializing.
    /// If a function is passed the source layer will be passed to the function.
    /// The function should return a <c>String</c> or <c>HTMLElement</c> to be used in the overlay.
    /// String content is rendered as HTML; sanitize untrusted input or pass an <c>HTMLElement</c> with safe <c>textContent</c> instead.
    /// </summary>
    /// <remarks>Default: <c>''</c></remarks>
    [J("content")] public string? Content { get; set; }
}
