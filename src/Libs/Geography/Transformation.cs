namespace Seedysoft.Libs.Geography;

public static class Transformation
{
    private const double EarthRadius = 6378137.0;
    private const double OriginShift = Math.PI * EarthRadius;

    public static (double X, double Y) LatLonToMercator(double lng, double lat)
    {
        double x = lng * OriginShift / 180.0;
        double y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) * EarthRadius;

        return new(x, y);
    }

    public static (double Lng, double Lat) MercatorToLatLon(double x, double y)
    {
        double lon = x / OriginShift * 180.0;
        double lat = y / OriginShift * 180.0;
        lat = 180 / Math.PI * ((2 * Math.Atan(Math.Exp(lat * Math.PI / 180.0))) - (Math.PI / 2));

        return new(lon, lat);
    }
}
