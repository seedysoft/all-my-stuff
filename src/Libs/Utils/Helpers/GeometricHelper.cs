namespace Seedysoft.Libs.Utils.Helpers;

public static class GeometricHelper
{
    private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180d);
    private static double RadiansToDegrees(double radians) => radians * (180d / Math.PI);

    private static double GetEarthRadiusKilometers(double latitudeInRadians)
    {
        double f1 = Math.Pow(Math.Pow(Core.Constants.Earth.RadiusMetersAtEquator, 2) * Math.Cos(latitudeInRadians), 2);
        double f2 = Math.Pow(Math.Pow(Core.Constants.Earth.RadiusMetersAtPole, 2) * Math.Sin(latitudeInRadians), 2);
        double f3 = Math.Pow(Core.Constants.Earth.RadiusMetersAtEquator * Math.Cos(latitudeInRadians), 2);
        double f4 = Math.Pow(Core.Constants.Earth.RadiusMetersAtPole * Math.Sin(latitudeInRadians), 2);

        double radius = Math.Sqrt((f1 + f2) / (f3 + f4));

        return radius / 1_000d;
    }

    public static double ExpandLongitude(double latInDegrees, double lonInDegrees, double kilometers)
    {
        // Length of 1 degree of longitude = cosine(latitude) * [length of degree at equator ]
        // www.colorado.edu/geography/gcraft/warmup/aquifer/html/distance.html [12/5/2000]

        double DeltaLongitude = kilometers / (double)(Math.Cos(DegreesToRadians(latInDegrees)) * GetOneDegreeLonKilometers(latInDegrees));

        return (double)Math.Round(lonInDegrees + DeltaLongitude, 5);

        static double GetOneDegreeLonKilometers(double latInDegrees)
        {
            // Compute spherical coordinates
            double EarthCircumferenceOnEquator = Core.Constants.Earth.RadiusMetersAtEquator / 1_000d * 2 * Math.PI;

            return (double)(Math.Cos(DegreesToRadians(latInDegrees)) * EarthCircumferenceOnEquator / Core.Constants.Earth.Degrees);
        }
    }
    public static double ExpandLatitude(double latInDegrees, double lonInDegrees, double kilometers)
    {
        // Length of 1 degree of latitude ranges from
        //    68.70 miles at  0 deg N to
        //    68.83          25 deg N (tip of Florida not including the Keys)
        //    69.12          50 deg N (approximate US/Canada border)   (range = 0.29 mile = 1531 feet = 510 yards)
        //    69.41 miles at 90 deg N (Compton's Encyclopedia Online v.3.0 www.comptons.com/encyclopedia/TABLES/150995113_T.html)
        //    68.703 miles at equator to 69.407 at poles due to earth's slightly ellipsoid shape)
        // "What is the distance between a degree of latitude and longitude?"
        // www.geography.about.com/science/geography/library/faq/blqzdistancedegree.htm cached 12/05/00

        (double MinLatInDegrees, double MinLonInDegrees, double MaxLatInDegrees, double MaxLonInDegrees)
            = GetOneDegreeLatKilometers(latInDegrees, lonInDegrees);

        double DeltaLatitude = kilometers / Haversine.Distance(MinLatInDegrees, MinLonInDegrees, MaxLatInDegrees, MaxLonInDegrees);

        return (double)Math.Round(latInDegrees + DeltaLatitude, 5);

        static (double MinLatInDegrees, double MinLonInDegrees, double MaxLatInDegrees, double MaxLonInDegrees) GetOneDegreeLatKilometers(
            double latInDegrees,
            double lonInDegrees,
            double halfSideInKm = 0)
        {
            double lat = DegreesToRadians(latInDegrees);
            double lon = DegreesToRadians(lonInDegrees);
            double halfSide = 500d * halfSideInKm;

            // Radius of Earth at given latitude
            double radius = GetEarthRadiusKilometers(latInDegrees);
            // Radius of the parallel at given latitude
            double pradius = radius * Math.Cos(lat);

            double latMin = lat - (halfSide / radius);
            double latMax = lat + (halfSide / radius);
            double lonMin = lon - (halfSide / pradius);
            double lonMax = lon + (halfSide / pradius);

            return (RadiansToDegrees(latMin), RadiansToDegrees(lonMin), RadiansToDegrees(latMax), RadiansToDegrees(lonMax));
        }
    }

    private static class Haversine
    {
        /// <summary>
        /// Returns the distance in kilometers of any two latitude / longitude points.
        /// </summary>
        /// <returns></returns>
        public static double Distance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            double Lat = DegreesToRadians(toLatitude - fromLatitude);
            double Lon = DegreesToRadians(toLongitude - fromLongitude);

            double a =
                (Math.Sin(Lat / 2) * Math.Sin(Lat / 2)) +
                (Math.Cos(DegreesToRadians(fromLatitude)) * Math.Cos(DegreesToRadians(toLatitude)) * Math.Sin(Lon / 2) * Math.Sin(Lon / 2));

            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

            return (double)(GetEarthRadiusKilometers(toLatitude) * c);
        }
    }
}
