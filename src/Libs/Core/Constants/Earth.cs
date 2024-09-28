namespace Seedysoft.Libs.Core.Constants;

public static class Earth
{
    public readonly record struct Home
    {
        public const double Lat = 42.354397413084406d;
        public const double Lng = -3.6628374207940815d;
    }

    public const double RadiusMeanInMeters = 6_371_008.8d;

    public const int Degrees = 360;
}
