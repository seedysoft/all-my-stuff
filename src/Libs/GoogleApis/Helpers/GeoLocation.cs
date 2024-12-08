//namespace Seedysoft.Libs.GoogleApis.Helpers;

///// <summary>
///// Represents a point on the surface of a sphere. (The Earth is almost spherical)
///// This code was originally published at http://JanMatuschek.de/LatitudeLongitudeBoundingCoordinates#Java
///// @author Jan Philip Matuschek
///// @version 22 September 2010
///// @converted to C# by Anthony Zigenbine on 19th October 2010
///// </summary>
//[System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay,nq}")]
//public class GeoLocation
//{
//    public double LatitudeInRadians { get; private set; }
//    public double LongitudeInRadians { get; private set; }
//    public double LatitudeInDegrees { get; private set; }
//    public double LongitudeInDegrees { get; private set; }

//    private static readonly double MaxLatitudeInRadians =
//        GeometricHelper.DegreesToRadians(Core.Constants.Earth.MaxLatitudeInDegrees); //  PI/2
//    private static readonly double MinLatitudeInRadians =
//        GeometricHelper.DegreesToRadians(Core.Constants.Earth.MinLatitudeInDegrees); // -PI/2
//    private static readonly double MaxLongitudeInRadians =
//        GeometricHelper.DegreesToRadians(Core.Constants.Earth.MaxLongitudeInDegrees); //  PI
//    private static readonly double MinLongitudeInRadians =
//        GeometricHelper.DegreesToRadians(Core.Constants.Earth.MinLongitudeInDegrees); // -PI

//    /// <summary>
//    /// Avoid creating instances outside.
//    /// </summary>
//    private GeoLocation() { }

//    /// <summary>
//    /// Return GeoLocation from Degrees
//    /// </summary>
//    /// <param name="latitude">The latitude, in degrees</param>
//    /// <param name="longitude">The longitude, in degrees</param>
//    /// <returns><see cref="GeoLocation"/> in degrees</returns>
//    public static GeoLocation FromDegrees(double latitude, double longitude)
//    {
//        GeoLocation result = new()
//        {
//            LatitudeInRadians = GeometricHelper.DegreesToRadians(latitude),
//            LongitudeInRadians = GeometricHelper.DegreesToRadians(longitude),
//            LatitudeInDegrees = latitude,
//            LongitudeInDegrees = longitude
//        };
//        result.CheckBounds();

//        return result;
//    }

//    /// <summary>
//    /// Return GeoLocation from Radians
//    /// </summary>
//    /// <param name="latitude">The latitude, in radians</param>
//    /// <param name="longitude">The longitude, in radians</param>
//    /// <returns><see cref="GeoLocation"/>, in radians</returns>
//    public static GeoLocation FromRadians(double latitude, double longitude)
//    {
//        GeoLocation result = new()
//        {
//            LatitudeInRadians = latitude,
//            LongitudeInRadians = longitude,
//            LatitudeInDegrees = GeometricHelper.RadiansToDegrees(latitude),
//            LongitudeInDegrees = GeometricHelper.RadiansToDegrees(longitude)
//        };
//        result.CheckBounds();

//        return result;
//    }

//    private void CheckBounds()
//    {
//        if (LatitudeInRadians < MinLatitudeInRadians ||
//            LatitudeInRadians > MaxLatitudeInRadians ||
//            LongitudeInRadians < MinLongitudeInRadians ||
//            LongitudeInRadians > MaxLongitudeInRadians)
//        {
//            throw new Exception("Arguments are out of bounds");
//        }
//    }

//    /// <summary>
//    /// Computes the great circle distance between this <see cref="GeoLocation"/> instance and the location argument
//    /// </summary>
//    /// <param name="location">Location to act as the centre point</param>
//    /// <returns>The distance, measured in the same unit as the radius argument</returns>
//    public double DistanceTo(GeoLocation location)
//    {
//        return
//            Math.Acos(
//                (Math.Sin(LatitudeInRadians) * Math.Sin(location.LatitudeInRadians))
//                + (Math.Cos(LatitudeInRadians) * Math.Cos(location.LatitudeInRadians) * Math.Cos(LongitudeInRadians - location.LongitudeInRadians))
//            ) * Core.Constants.Earth.MeanRadiusInMeters / 1_000;
//    }

//    /// <summary>
//    /// Computes the bounding coordinates of all points on the surface of a sphere that have a great circle distance to the point represented by this GeoLocation instance that is less or equal to the distance argument.
//    /// For more information about the formulae used in this method visit http://JanMatuschek.de/LatitudeLongitudeBoundingCoordinates.
//    /// </summary>
//    /// <param name="distanceInKilometers">The distance from the point represented by this GeoLocation instance.
//    /// Must me measured in the same unit as the radius argument</param>
//    /// <returns>An array of two GeoLocation objects such that:
//    /// a) The latitude of any point within the specified distance is greater or equal to the latitude of the first array element and smaller or equal to the latitude of the second array element.
//    /// If the longitude of the first array element is smaller or equal to the longitude of the second element, then the longitude of any point within the specified distance is greater or equal to the longitude of the first array element and smaller or equal to the longitude of the second array element.
//    /// 
//    /// b) If the longitude of the first array element is greater than the longitude of the second element (this is the case if the 180th meridian is within the distance), then the longitude of any point within the specified distance is greater or equal to the longitude of the first array element
//    /// or smaller or equal to the longitude of the second array element.</returns>
//    public GeoLocation[] BoundingCoordinates(double distanceInKilometers)
//    {
//        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(distanceInKilometers);

//        // angular distance in radians on a great circle
//        double radDist = distanceInKilometers / Core.Constants.Earth.MeanRadiusInMeters / 1_000;

//        double minLatInRadians = LatitudeInRadians - radDist;
//        double maxLatInRadians = LatitudeInRadians + radDist;

//        double minLonInRadians;
//        double maxLonInRadians;
//        if (minLatInRadians > MinLatitudeInRadians && maxLatInRadians < MaxLatitudeInRadians)
//        {
//            double deltaLon = Math.Asin(Math.Sin(radDist) / Math.Cos(LatitudeInRadians));
//            minLonInRadians = LongitudeInRadians - deltaLon;
//            if (minLonInRadians < MinLongitudeInRadians)
//                minLonInRadians += 2d * Math.PI;
//            maxLonInRadians = LongitudeInRadians + deltaLon;
//            if (maxLonInRadians > MaxLongitudeInRadians)
//                maxLonInRadians -= 2d * Math.PI;
//        }
//        else
//        {
//            // a pole is within the distance
//            minLatInRadians = Math.Max(minLatInRadians, MinLatitudeInRadians);
//            maxLatInRadians = Math.Min(maxLatInRadians, MaxLatitudeInRadians);
//            minLonInRadians = MinLongitudeInRadians;
//            maxLonInRadians = MaxLongitudeInRadians;
//        }

//        return
//        [
//            FromRadians(minLatInRadians, minLonInRadians),
//            FromRadians(maxLatInRadians, maxLonInRadians)
//        ];
//    }

//    private string GetDebuggerDisplay =>
//        $"Lat: {LatitudeInDegrees} \u00B0 {Environment.NewLine}" +
//        $"Lon: {LongitudeInDegrees} \u00B0 {Environment.NewLine}" +
//        $"Lat: {LatitudeInRadians} rad {Environment.NewLine}" +
//        $"Lon: {LongitudeInRadians} rad";
//}
