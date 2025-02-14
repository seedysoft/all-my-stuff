namespace Seedysoft.Libs.GoogleApis.Helpers;

public static class GeometricHelper
{
    public static double DegreesToRadians(double degrees) => degrees * (Math.PI / Core.Constants.Earth.TotalDegrees / 2);
    public static double RadiansToDegrees(double radians) => radians * (Core.Constants.Earth.TotalDegrees / 2 / Math.PI);

    public static Models.Shared.LatLngBoundsLiteral GetBounds(List<Models.Shared.LatLngLiteral> routePoints, int maxDistanceInKm)
    {
        double North = routePoints.Max(x => x.Lat);
        double South = routePoints.Min(x => x.Lat);
        double East = routePoints.Max(x => x.Lng);
        double West = routePoints.Min(x => x.Lng);

        double LatExpanded = ExpandLatitude(North, West, maxDistanceInKm);
        double LngExpanded = ExpandLongitude(South, East, maxDistanceInKm);

        return new Models.Shared.LatLngBoundsLiteral()
        {
            North = LatExpanded,
            South = South - (LatExpanded - South),
            East = LngExpanded,
            West = West - (LngExpanded - West),
        };
    }

    public static double ExpandLongitude(double latDegrees, double lonDegrees, double kilometers)
    {
        // Length of 1 degree of longitude = cosine(latitude) * [length of degree at equator]
        // www.colorado.edu/geography/gcraft/warmup/aquifer/html/distance.html [12/5/2000]

        double DeltaLongitude = kilometers / (Math.Cos(DegreesToRadians(latDegrees)) * GetOneDegreeLngKilometers(latDegrees));

        return lonDegrees + DeltaLongitude;

        static double GetOneDegreeLngKilometers(double latDegrees)
        {
            // Compute spherical coordinates
            double EarthCircumferenceOnEquatorInKilometers = Core.Constants.Earth.MeanRadiusInKilometers * 2 * Math.PI;

            return Math.Cos(DegreesToRadians(latDegrees)) * EarthCircumferenceOnEquatorInKilometers / Core.Constants.Earth.TotalDegrees;
        }
    }
    public static double ExpandLatitude(double latDegrees, double lonDegrees, double kilometers)
    {
        // Length of 1 degree of latitude ranges from
        //    68.70 miles at  0 deg N to
        //    68.83          25 deg N (tip of Florida not including the Keys)
        //    69.12          50 deg N (approximate US/Canada border)   (range = 0.29 mile = 1531 feet = 510 yards)
        //    69.41 miles at 90 deg N (Compton's Encyclopedia Online v.3.0 www.comptons.com/encyclopedia/TABLES/150995113_T.html)
        //    68.703 miles at equator to 69.407 at poles due to earth's slightly ellipsoid shape)
        // "What is the distance between a degree of latitude and longitude?"
        // www.geography.about.com/science/geography/library/faq/blqzdistancedegree.htm cached 12/05/00

        Models.Shared.LatLngBoundsLiteral LatLngBounds = GetOneDegreeLatKilometers(latDegrees, lonDegrees, kilometers);

        double DeltaLatitude = kilometers / Haversine.Distance(LatLngBounds.South, LatLngBounds.West, LatLngBounds.North, LatLngBounds.East);

        return latDegrees + DeltaLatitude;

        static Models.Shared.LatLngBoundsLiteral GetOneDegreeLatKilometers(
            double latDegrees,
            double lonDegrees,
            double halfSideInKm = 0)
        {
            // Radius of Earth at given latitude
            double latRadians = DegreesToRadians(latDegrees);
            // Radius of the parallel at given latitude
            double pRadius = Core.Constants.Earth.MeanRadiusInKilometers * Math.Cos(latRadians);

            double halfSideInMeters = 500d * halfSideInKm;

            double HalfSize = halfSideInMeters / Core.Constants.Earth.MeanRadiusInKilometers;
            double latRadiansMin = latRadians - HalfSize;
            double latRadiansMax = latRadians + HalfSize;

            double lonRadians = DegreesToRadians(lonDegrees);

            double HalfRadius = halfSideInMeters / pRadius;
            double lonRadiansMin = lonRadians - HalfRadius;
            double lonRadiansMax = lonRadians + HalfRadius;

            return new()
            {
                North = RadiansToDegrees(latRadiansMax),
                South = RadiansToDegrees(latRadiansMin),
                East = RadiansToDegrees(lonRadiansMax),
                West = RadiansToDegrees(lonRadiansMin),
            };
        }
    }

    public static class Haversine
    {
        /// <summary>
        /// Returns the distance in kilometers of any two latitude / longitude points.
        /// </summary>
        /// <param name="fromLatDegrees">From point latitude</param>
        /// <param name="fromLngDegrees">From point longitude</param>
        /// <param name="toLatDegrees">To point latitude</param>
        /// <param name="toLngDegrees">To point longitude</param>
        /// <returns></returns>
        public static double Distance(double fromLatDegrees, double fromLngDegrees, double toLatDegrees, double toLngDegrees)
        {
            double LatRadians = DegreesToRadians(toLatDegrees - fromLatDegrees);
            double LngRadians = DegreesToRadians(toLngDegrees - fromLngDegrees);

            double ToLatRadians = DegreesToRadians(toLatDegrees);
            double FromLatRadians = DegreesToRadians(fromLatDegrees);

            double a =
                (Math.Sin(LatRadians / 2) * Math.Sin(LatRadians / 2)) +
                (Math.Cos(FromLatRadians) * Math.Cos(ToLatRadians) * Math.Sin(LngRadians / 2) * Math.Sin(LngRadians / 2));

            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

            double DistanceInKilometers = (double)(Core.Constants.Earth.MeanRadiusInKilometers * c);
            
            return DistanceInKilometers;
        }
    }

    /// <summary>
    /// Haversine formula to calculate great-circle (orthodromic) distance on Earth.
    /// High Accuracy, Medium speed.
    /// </summary>
    /// <param name="from">LatLngLiteral: 1st point</param>
    /// <param name="to">LatLngLiteral: 2nd point</param>
    /// <returns>double: distance in kilometers</returns>
    public static double DistanceHaversineInKilometers(Models.Shared.LatLngLiteral from, Models.Shared.LatLngLiteral to)
    {
        double _radLat1 = DegreesToRadians(from.Lat);
        double _radLat2 = DegreesToRadians(to.Lat);

        double _dLatHalf = (_radLat2 - _radLat1) / 2;
        double _dLonHalf = Math.PI * (to.Lng - from.Lng) / Core.Constants.Earth.TotalDegrees;

        // intermediate result
        double _a = Math.Sin(_dLatHalf);
        _a *= _a;

        // intermediate result
        double _b = Math.Sin(_dLonHalf);
        _b *= _b * Math.Cos(_radLat1) * Math.Cos(_radLat2);

        // central angle, aka arc segment angular distance
        double _centralAngle = 2 * Math.Atan2(Math.Sqrt(_a + _b), Math.Sqrt(1 - _a - _b));

        // great-circle (orthodromic) distance on Earth between 2 points
        double DistanceInKilometers = Core.Constants.Earth.MeanRadiusInKilometers * _centralAngle;

        return DistanceInKilometers;
    }
}
