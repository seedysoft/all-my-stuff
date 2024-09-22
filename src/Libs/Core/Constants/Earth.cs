namespace Seedysoft.Libs.Core.Constants;

public static class Earth
{
    public readonly record struct Home(double Lat = 42.354397413084406, double Lon = -3.6628374207940815);

    public const double RadiusMetersAtEquator = 6_378_137.000d;
    public const double RadiusMetersAtPole = 6_356_752.3142d;

    //public const double PerimeterInKilometers = 2 * Math.PI * RadiusInKilometers; //40075.017D;

    public const int Degrees = 360;
}

// Decimal places  Decimal degrees  Distance (meters)  Notes
// 0               1.0              110_574.3          111 km
// 1               0.1               11_057.43          11 km
// 2               0.01               1_105.74           1 km
// 3               0.001                110.57       0.111 km
// 4               0.0001                11.06        1106 cm
// 5               0.00001                1.11         111 cm
// 6               0.000001               0.11          11 cm
// 7               0.0000001              0.01           1 cm
// 8               0.00000001             0.001          1 mm
