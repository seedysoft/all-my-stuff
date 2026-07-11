namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public sealed class TileLayer : ILayer
{
    public string Id { get; }

    internal TileLayer(string id) => Id = id;
}
