namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public sealed class Polygon : ILayer
{
    public string Id { get; }
    public IReadOnlyList<LatLng> Points { get; }

    internal Polygon(string id, IReadOnlyList<LatLng> points)
    {
        Id = id;
        Points = points;
    }
}
