namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public sealed class Circle : ILayer
{
    public string Id { get; }
    public LatLng Center { get; }
    public double Radius { get; }

    internal Circle(string id, LatLng center, double radius)
    {
        Id = id;
        Center = center;
        Radius = radius;
    }
}
