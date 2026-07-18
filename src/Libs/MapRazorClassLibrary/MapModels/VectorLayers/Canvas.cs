namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.VectorLayers;

/// <summary>
/// Allows vector layers to be displayed with <canvas>.
/// Inherits <see cref="Base.Renderer"/>.
/// </summary>
public sealed record class Canvas : Base.Renderer
{
    public Canvas() : base() { }

    /// <summary>
    /// How much to extend the click tolerance around a path/object on the map.
    /// </summary>
    /// <remarks>Default: 0</remarks>
    [J("tolerance")] public double? Tolerance { get; set; } = 0;
}
