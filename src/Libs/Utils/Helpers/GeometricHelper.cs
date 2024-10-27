//namespace Seedysoft.Libs.Utils.Helpers;

//public static class GeometricHelper
//{
//    public static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180d);
//    public static double RadiansToDegrees(double radians) => radians * (180d / Math.PI);

//    public static double ExpandLongitude(double latDegrees, double lonDegrees, double kilometers)
//    {
//        // Length of 1 degree of longitude = cosine(latitude) * [length of degree at equator]
//        // www.colorado.edu/geography/gcraft/warmup/aquifer/html/distance.html [12/5/2000]

//        double DeltaLongitude = kilometers / (double)(Math.Cos(DegreesToRadians(latDegrees)) * GetOneDegreeLonKilometers(latDegrees));

//        return (double)Math.Round(lonDegrees + DeltaLongitude, 5);

//        static double GetOneDegreeLonKilometers(double latDegrees)
//        {
//            // Compute spherical coordinates
//            double EarthCircumferenceOnEquator = Core.Constants.Earth.RadiusMeanInMeters / 1_000d * 2 * Math.PI;

//            return (double)(Math.Cos(DegreesToRadians(latDegrees)) * EarthCircumferenceOnEquator / Core.Constants.Earth.Degrees);
//        }
//    }
//    public static double ExpandLatitude(double latDegrees, double lonDegrees, double kilometers)
//    {
//        // Length of 1 degree of latitude ranges from
//        //    68.70 miles at  0 deg N to
//        //    68.83          25 deg N (tip of Florida not including the Keys)
//        //    69.12          50 deg N (approximate US/Canada border)   (range = 0.29 mile = 1531 feet = 510 yards)
//        //    69.41 miles at 90 deg N (Compton's Encyclopedia Online v.3.0 www.comptons.com/encyclopedia/TABLES/150995113_T.html)
//        //    68.703 miles at equator to 69.407 at poles due to earth's slightly ellipsoid shape)
//        // "What is the distance between a degree of latitude and longitude?"
//        // www.geography.about.com/science/geography/library/faq/blqzdistancedegree.htm cached 12/05/00

//        Core.Models.LatLngBoundsLiteral LatLngBounds = GetOneDegreeLatKilometers(latDegrees, lonDegrees, kilometers);

//        double DeltaLatitude = kilometers / Haversine.Distance(LatLngBounds.South, LatLngBounds.West, LatLngBounds.North, LatLngBounds.East);

//        return (double)Math.Round(latDegrees + DeltaLatitude, 5);

//        static Core.Models.LatLngBoundsLiteral GetOneDegreeLatKilometers(
//            double latDegrees,
//            double lonDegrees,
//            double halfSideInKm = 0)
//        {
//            // Radius of Earth at given latitude
//            double radius = Core.Constants.Earth.RadiusMeanInMeters / 1_000;

//            double latRadians = DegreesToRadians(latDegrees);
//            // Radius of the parallel at given latitude
//            double pradius = radius * Math.Cos(latRadians);

//            double halfSide = 500d * halfSideInKm;

//            double latRadiansMin = latRadians - (halfSide / radius);
//            double latRadiansMax = latRadians + (halfSide / radius);

//            double lonRadians = DegreesToRadians(lonDegrees);

//            double lonRadiansMin = lonRadians - (halfSide / pradius);
//            double lonRadiansMax = lonRadians + (halfSide / pradius);

//            return new()
//            {
//                North = RadiansToDegrees(latRadiansMax),
//                South = RadiansToDegrees(latRadiansMin),
//                East = RadiansToDegrees(lonRadiansMax),
//                West = RadiansToDegrees(lonRadiansMin),
//            };
//        }
//    }

//    private static class Haversine
//    {
//        /// <summary>
//        /// Returns the distance in kilometers of any two latitude / longitude points.
//        /// </summary>
//        /// <returns></returns>
//        public static double Distance(double fromLatDegrees, double fromLonDegrees, double toLatDegrees, double toLonDegrees)
//        {
//            double LatRadians = DegreesToRadians(toLatDegrees - fromLatDegrees);
//            double LonRadians = DegreesToRadians(toLonDegrees - fromLonDegrees);

//            double a =
//                (Math.Sin(LatRadians / 2) * Math.Sin(LatRadians / 2)) +
//                (Math.Cos(DegreesToRadians(fromLatDegrees)) * Math.Cos(DegreesToRadians(toLatDegrees)) * Math.Sin(LonRadians / 2) * Math.Sin(LonRadians / 2));

//            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

//            return (double)(Core.Constants.Earth.RadiusMeanInMeters / 1_000 * c);
//        }
//    }
//}
