namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.OtherLayers;

/// <summary>
/// Generic class for handling a tiled grid of HTML elements.
/// his is the base class for all tile layers and replaces <c>TileLayer.Canvas</c>>.
/// GridLayer can be extended to create a tiled grid of HTML elements like <c>&lt;canvas&gt;</c>, <c>&lt;img&gt;</c> or <c>&lt;div&gt;</c>.
/// GridLayer will handle creating and animating these DOM elements for you.
/// </summary>
public record class GridLayer : Base.Layer
{
    /// <summary>
    /// Width and height of tiles in the grid.
    /// Use a number if width and height are equal, or <see cref="Basic.Point">(<c>width</c>, <c>height</c>) otherwise.
    /// </summary>
    /// <remarks>Default: <c>256</c></remarks>
    public OneOf.OneOf<double, Basic.Point>? TileSize { get; set; } = 256;
}
