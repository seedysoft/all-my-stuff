namespace Seedysoft.Libs.Travel.Helpers;

public class CoordinatesCalculatesHelper
{
    private const double RADIO_EARTH_ECUATORIAL_IN_METTERS = 6_378_137;

    //private static double DEGREES_PER_METTER_OF_LATTITUDE => 360 / (2 * Math.PI * RADIO_EARTH_ECUATORIAL_IN_METTERS);

    //public static double GetLatitudeFromDegreesPerMetter(double latitude, double angle, double distanceInMetters)
    //{
    //    double y = Math.Sin(angle) * distanceInMetters;

    //    return latitude + (y * DEGREES_PER_METTER_OF_LATTITUDE);
    //}

    //public static double GetLongitudeFromDegreesPerMetter(double latitude, double longitude, double angle, double distanceInMetters)
    //{
    //    double x = Math.Cos(angle) * distanceInMetters;
    //    double longitudeGradesToAdd = x * DEGREES_PER_METTER_OF_LATTITUDE;
    //    longitudeGradesToAdd /= Math.Cos(latitude * (Math.PI / 180));

    //    return longitude + longitudeGradesToAdd;
    //}

    public static double CalculateDistanceInMetters(Models.Location origin, Models.Location destination)
    {
        double origLatToRad = GeoMathHelper.ConvertToRadians(origin.Latitude);
        double origLonToRad = GeoMathHelper.ConvertToRadians(origin.Longitude);
        double destLatToRad = GeoMathHelper.ConvertToRadians(destination.Latitude);
        double destLonToRad = GeoMathHelper.ConvertToRadians(destination.Longitude);
        double haversine = GeoMathHelper.CalculateHaversine(origLatToRad, origLonToRad, destLatToRad, destLonToRad);
        double result = RADIO_EARTH_ECUATORIAL_IN_METTERS * haversine;

        return result;
    }
}
