namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.VectorLayers;

/// <summary>
/// Allows vector layers to be displayed with <canvas>.
/// Inherits <see cref="Base.Renderer"/>.
/// </summary>
internal sealed class Canvas : Base.Renderer
{
    public Canvas() : base() { }

    /// <summary>
    /// How much to extend the click tolerance around a path/object on the map.
    /// </summary>
    /// <remarks>Default: <c>0</c></remarks>
    [J("tolerance")] public double? Tolerance { get; set; } = 0;
}
