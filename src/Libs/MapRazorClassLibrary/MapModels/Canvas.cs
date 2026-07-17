namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// Allows vector layers to be displayed with <canvas>.
/// Inherits <see cref="Renderer"/>.
/// </summary>
public record class Canvas : Renderer
{
    public Canvas() : base() { }

    /// <summary>
    /// How much to extend the click tolerance around a path/object on the map.
    /// </summary>
    /// <remarks>Default: 0</remarks>
    [J("tolerance")] public double? Tolerance { get; set; } = 0;
}
