namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public record class MarkerPosition
{
    public MarkerPosition(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    [J("lat")] public double Latitude { get; }

    [J("lng")] public double Longitude { get; }
}
