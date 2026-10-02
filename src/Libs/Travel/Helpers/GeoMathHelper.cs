namespace Seedysoft.Libs.Travel.Helpers;

public class GeoMathHelper
{
    public static double ConvertToRadians(double grados) => grados * (Math.PI / 180.0);
    public static double ConvertToGrades(double rad) => rad * (180.0 / Math.PI);

    public static double CalculateHaversine(
        double originLatInRad,
        double originLonInRad,
        double destinationLatInRad,
        double destinationLonInRad)
    {
        double difLat = destinationLatInRad - originLatInRad;
        double difLon = destinationLonInRad - originLonInRad;

        //haversine
        double angle = (Math.Sin(difLat / 2) * Math.Sin(difLat / 2)) +
           (Math.Cos(originLatInRad) * Math.Cos(destinationLatInRad) * Math.Sin(difLon / 2) * Math.Sin(difLon / 2));

        double haversine = 2 * Math.Atan2(Math.Sqrt(angle), Math.Sqrt(1 - angle));

        return haversine;
    }

    //public static double CalculateRandomAngleInRadians()
    //{
    //    Random random = new();
    //    double randomAngle = random.NextDouble() * 2 * Math.PI;

    //    return randomAngle;
    //}

    //public static double CalculateRandomDegrees() => new Random().Next(0, 360);

    //public static double CalculateRandomDistance(double distance)
    //{
    //    Random random = new();
    //    int randomNum = random.Next(1, 10);
    //    bool toLeft = randomNum % 2 == 0;
    //    double sqrt = Math.Sqrt(random.NextDouble());
    //    if (toLeft)
    //        sqrt *= -1;
    //    double result = distance * sqrt;

    //    return result;
    //}
}
