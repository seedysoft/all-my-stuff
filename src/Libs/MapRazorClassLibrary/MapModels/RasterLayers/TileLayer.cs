namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.RasterLayers;

/// <summary>
/// Used to load and display tile layers on the map. Note that most tile servers require attribution, which you can set under <see cref="Base.Layer"/>.
/// Extends <see cref="OtherLayers.GridLayer"/>.
/// </summary>
public sealed record class TileLayer : OtherLayers.GridLayer
{
}
