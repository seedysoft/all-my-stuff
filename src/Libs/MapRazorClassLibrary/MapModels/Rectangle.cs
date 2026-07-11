namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public sealed class Rectangle : ILayer
{
    public string Id { get; }
    public LatLng SouthWest { get; }
    public LatLng NorthEast { get; }

    internal Rectangle(string id, LatLng sw, LatLng ne)
    {
        Id = id;
        SouthWest = sw;
        NorthEast = ne;
    }
}
