namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public sealed class Marker : ILayer
{
    public string Id { get; }
    public LatLng Position { get; }

    internal Marker(string id, LatLng position)
    {
        Id = id;
        Position = position;
    }
}

public sealed class MarkerOptions
{
    [J("title")] public string? Title { get; set; }
}
