namespace Seedysoft.Photo.Lib.Exif;

public struct GeoCoordinate
{
    private const char CardinalPointEast = 'E';
    private const char CardinalPointNorth = 'N';
    private const char CardinalPointSouth = 'S';
    private const char CardinalPointWest = 'W';

    // Integer number: For latitudes: 0 ≤ Degree ≤ 90 
    // For longitudes: 0 ≤ Degree ≤ 180
    public decimal Degree { get; set; }
    // Integer number: 0 ≤ Minute < 60
    public decimal Minute { get; set; }
    // Fraction number: 0 ≤ Second < 60
    public decimal Second { get; set; }
    // For latitudes: 'N' or 'S'; for longitudes: 'E' or 'W'
    public char CardinalPoint { get; set; }

    public static decimal ToDecimal(GeoCoordinate value)
    {
        decimal DecimalDegree = value.Degree + (value.Minute / 60) + (value.Second / 3600);

        return value.CardinalPoint is CardinalPointNorth or CardinalPointEast ? DecimalDegree : -DecimalDegree;
    }

    public static GeoCoordinate FromDecimal(decimal value, bool isLatitude)
    {
        decimal AbsValue;
        GeoCoordinate ret = new();

        if (value >= 0)
        {
            ret.CardinalPoint = isLatitude ? CardinalPointNorth : CardinalPointEast;
            AbsValue = value;
        }
        else
        {
            ret.CardinalPoint = isLatitude ? CardinalPointSouth : CardinalPointWest;
            AbsValue = -value;
        }

        ret.Degree = decimal.Truncate(AbsValue);
        decimal frac = (AbsValue - ret.Degree) * 60;
        ret.Minute = decimal.Truncate(frac);
        ret.Second = (frac - ret.Minute) * 60;

        return ret;
    }
}
