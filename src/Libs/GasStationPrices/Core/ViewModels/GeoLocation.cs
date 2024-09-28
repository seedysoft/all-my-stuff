namespace Seedysoft.Libs.GasStationPrices.Core.ViewModels;

/// <summary>
/// Represents a point on the surface of a sphere. (The Earth is almost spherical)
/// This code was originally published at http://JanMatuschek.de/LatitudeLongitudeBoundingCoordinates#Java
/// @author Jan Philip Matuschek
/// @version 22 September 2010
/// @converted to C# by Anthony Zigenbine on 19th October 2010
/// </summary>
public class GeoLocation
{
    public double LatitudeInRadians { get; private set; }
    public double LongitudeInRadians { get; private set; }
    public double LatitudeInDegrees { get; private set; }
    public double LongitudeInDegrees { get; private set; }

    private static readonly double MIN_LAT_RAD = Utils.Helpers.GeometricHelper.DegreesToRadians(-90d);  // -PI/2
    private static readonly double MAX_LAT_RAD = Utils.Helpers.GeometricHelper.DegreesToRadians(90d);   //  PI/2
    private static readonly double MIN_LON_RAD = Utils.Helpers.GeometricHelper.DegreesToRadians(-180d); // -PI
    private static readonly double MAX_LON_RAD = Utils.Helpers.GeometricHelper.DegreesToRadians(180d);  //  PI

    private GeoLocation() { }

    /// <summary>
    /// Return GeoLocation from Degrees
    /// </summary>
    /// <param name="latitude">The latitude, in degrees.</param>
    /// <param name="longitude">The longitude, in degrees.</param>
    /// <returns>GeoLocation in Degrees</returns>
    public static GeoLocation FromDegrees(double latitude, double longitude)
    {
        GeoLocation result = new()
        {
            LatitudeInRadians = Utils.Helpers.GeometricHelper.DegreesToRadians(latitude),
            LongitudeInRadians = Utils.Helpers.GeometricHelper.DegreesToRadians(longitude),
            LatitudeInDegrees = latitude,
            LongitudeInDegrees = longitude
        };
        result.CheckBounds();

        return result;
    }

    /// <summary>
    /// Return GeoLocation from Radians
    /// </summary>
    /// <param name="latitude">The latitude, in radians.</param>
    /// <param name="longitude">The longitude, in radians.</param>
    /// <returns>GeoLocation in Radians</returns>
    public static GeoLocation FromRadians(double latitude, double longitude)
    {
        GeoLocation result = new()
        {
            LatitudeInRadians = latitude,
            LongitudeInRadians = longitude,
            LatitudeInDegrees = Utils.Helpers.GeometricHelper.RadiansToDegrees(latitude),
            LongitudeInDegrees = Utils.Helpers.GeometricHelper.RadiansToDegrees(longitude)
        };
        result.CheckBounds();

        return result;
    }

    private void CheckBounds()
    {
        if (LatitudeInRadians < MIN_LAT_RAD || LatitudeInRadians > MAX_LAT_RAD || LongitudeInRadians < MIN_LON_RAD || LongitudeInRadians > MAX_LON_RAD)
            throw new Exception("Arguments are out of bounds");
    }

    public override string ToString() => $"({LatitudeInDegrees}\u00B0, {LongitudeInDegrees}\u00B0) = ({LatitudeInRadians} rad, {LongitudeInRadians} rad)";

    /// <summary>
    /// Computes the great circle distance between this GeoLocation instance and the location argument.
    /// </summary>
    /// <param name="location">Location to act as the centre point</param>
    /// <returns>the distance, measured in the same unit as the radius argument.</returns>
    public double DistanceTo(GeoLocation location)
    {
        return Math.Acos((Math.Sin(LatitudeInRadians) * Math.Sin(location.LatitudeInRadians)) +
                (Math.Cos(LatitudeInRadians) * Math.Cos(location.LatitudeInRadians) * Math.Cos(LongitudeInRadians - location.LongitudeInRadians)))
            * Libs.Core.Constants.Earth.RadiusMeanInMeters / 1_000;
    }

    /// <summary>
    /// Computes the bounding coordinates of all points on the surface of a sphere that have a great circle distance to the point represented by this GeoLocation instance that is less or equal to the distance argument.
    /// For more information about the formulae used in this method visit http://JanMatuschek.de/LatitudeLongitudeBoundingCoordinates.
    /// </summary>
    /// <param name="distance">The distance from the point represented by this GeoLocation instance.
    /// Must me measured in the same unit as the radius argument.</param>
    /// <returns>An array of two GeoLocation objects such that:
    /// a) The latitude of any point within the specified distance is greater or equal to the latitude of the first array element and smaller or equal to the latitude of the second array element.
    /// If the longitude of the first array element is smaller or equal to the longitude of the second element, then the longitude of any point within the specified distance is greater or equal to the longitude of the first array element and smaller or equal to the longitude of the second array element.
    /// 
    /// b) If the longitude of the first array element is greater than the longitude of the second element (this is the case if the 180th meridian is within the distance), then the longitude of any point within the specified distance is greater or equal to the longitude of the first array element
    /// or smaller or equal to the longitude of the second array element.</returns>
    public GeoLocation[] BoundingCoordinates(double distance)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(distance);

        // angular distance in radians on a great circle
        double radDist = distance / Libs.Core.Constants.Earth.RadiusMeanInMeters / 1_000;

        double minLat = LatitudeInRadians - radDist;
        double maxLat = LatitudeInRadians + radDist;

        double minLon;
        double maxLon;
        if (minLat > MIN_LAT_RAD && maxLat < MAX_LAT_RAD)
        {
            double deltaLon = Math.Asin(Math.Sin(radDist) / Math.Cos(LatitudeInRadians));
            minLon = LongitudeInRadians - deltaLon;
            if (minLon < MIN_LON_RAD)
                minLon += 2d * Math.PI;
            maxLon = LongitudeInRadians + deltaLon;
            if (maxLon > MAX_LON_RAD)
                maxLon -= 2d * Math.PI;
        }
        else
        {
            // a pole is within the distance
            minLat = Math.Max(minLat, MIN_LAT_RAD);
            maxLat = Math.Min(maxLat, MAX_LAT_RAD);
            minLon = MIN_LON_RAD;
            maxLon = MAX_LON_RAD;
        }

        return
        [
            FromRadians(minLat, minLon),
            FromRadians(maxLat, maxLon)
        ];
    }
}
