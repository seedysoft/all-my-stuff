namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Controls;

/// <summary>
/// The layers control gives users the ability to switch between different base layers and switch overlays on/off (check out the detailed example).
/// Extends <see cref="Base.Control"/>.
/// </summary>
internal sealed class Layers : Base.Control
{
    public Layers() : base() => Position = Positions.TopRight;

    /// <summary>
    /// If <c>true</c>, the control will be collapsed into an icon and expanded on pointer hover, touch, or keyboard activation.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("collapsed")] public bool? Collapsed { get; set; } = true;

    /// <summary>
    /// Collapse delay in milliseconds.
    /// If greater than 0, the control will remain open longer, making it easier to scroll through long layer lists.
    /// </summary>
    /// <remarks>Default: <c>0</c></remarks>
    [J("collapseDelay")] public double? CollapseDelay { get; set; } = 0;

    /// <summary>
    /// If <c>true</c>, the control will assign zIndexes in increasing order to all of its layers so that the order is preserved when switching them on/off.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("autoZIndex")] public bool? AutoZIndex { get; set; } = true;

    /// <summary>
    /// If <c>true</c>, the base layers in the control will be hidden when there is only one.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("hideSingleBase")] public bool? HideSingleBase { get; set; } = false;

    /// <summary>
    /// Whether to sort the layers.
    /// When <c>false</c>, layers will keep the order in which they were added to the control.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("sortLayers")] public bool? SortLayers { get; set; } = false;

    /// <summary>
    /// A compare function that will be used for sorting the layers, when sortLayers is true.
    /// The function receives both the <see cref="Base.Layer"/> instances and their names, as in sortFunction(layerA, layerB, nameA, nameB).
    /// By default, it sorts layers alphabetically by their name.
    /// </summary>
    /// <remarks>Default: <c>*</c></remarks>
    [J("sortFunction")] public string? SortFunction { get; set; }
}
