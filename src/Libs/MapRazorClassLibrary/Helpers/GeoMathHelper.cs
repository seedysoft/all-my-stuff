namespace Seedysoft.Libs.MapRazorClassLibrary.Helpers;

public class GeoMathHelper
{
    //public static double CalculateRandomAngleInRadians()
    //{
    //    Random random = new();
    //    double randomAngle = random.NextDouble() * 2 * Math.PI;

    //    return randomAngle;
    //}

    //public static double CalculateRandomDegrees() => new Random().Next(0, 360);

    //public static double ConvertToRadians(double grados) => grados * (Math.PI / 180.0);

    //public static double ConvertToGrades(double rad) => rad * (180.0 / Math.PI);

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

    //public static double CalculateHaversine(
    //    double originLatInRad,
    //    double originLongitudInRad,
    //    double destinationLatiInRad,
    //    double destinationLongitudeInRad)
    //{
    //    double difLatitud = CalculateDiference(destinationLatiInRad, originLatInRad);
    //    double difLongitud = CalculateDiference(destinationLongitudeInRad, originLongitudInRad);

    //    //haversine
    //    double angle = (Math.Sin(difLatitud / 2) * Math.Sin(difLatitud / 2)) +
    //               (Math.Cos(originLatInRad) * Math.Cos(destinationLatiInRad) *
    //               Math.Sin(difLongitud / 2) * Math.Sin(difLongitud / 2));
    //    double haversine = 2 * Math.Atan2(Math.Sqrt(angle), Math.Sqrt(1 - angle));

    //    return haversine;
    //}

    //private static double CalculateDiference(double firstValue, double secondValue) => firstValue - secondValue;
}
