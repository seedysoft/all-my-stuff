namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public sealed class Polyline : ILayer
{
    public string Id { get; }
    public IReadOnlyList<LatLng> Points { get; }

    internal Polyline(string id, IReadOnlyList<LatLng> points)
    {
        Id = id;
        Points = points;
    }
}
