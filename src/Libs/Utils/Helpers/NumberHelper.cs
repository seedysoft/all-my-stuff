namespace Seedysoft.Libs.Utils.Helpers;

public static class NumberHelper
{
    public static string ToReadableDistance(double? meters, System.Globalization.CultureInfo? cultureInfo = null)
    {
        if (!meters.HasValue)
            return "0 m";

        double UnsignedMeters = Math.Abs(meters.Value);

        cultureInfo ??= System.Globalization.CultureInfo.InvariantCulture;

        return UnsignedMeters switch
        {
            >= 10_000 => $"{(UnsignedMeters / 1_000).ToString("0", cultureInfo)} km",
            > 997 => $"{(UnsignedMeters / 1_000).ToString("0.0", cultureInfo)} km",
            < 1 => $"{UnsignedMeters * 100:0} cm",
            _ => $"{UnsignedMeters:0} m",
        };
    }
    public static string ToReadableDistance(float? meters, System.Globalization.CultureInfo? cultureInfo = null)
    {
        if (!meters.HasValue)
            return "0 m";

        float UnsignedMeters = Math.Abs(meters.Value);

        cultureInfo ??= System.Globalization.CultureInfo.InvariantCulture;

        return UnsignedMeters switch
        {
            >= 10_000 => $"{(UnsignedMeters / 1_000).ToString("0", cultureInfo)} km",
            > 997 => $"{(UnsignedMeters / 1_000).ToString("0.0", cultureInfo)} km",
            < 1 => $"{UnsignedMeters * 100:0} cm",
            _ => $"{UnsignedMeters:0} m",
        };
    }
    public static string ToReadableDistance(int? meters, System.Globalization.CultureInfo? cultureInfo = null)
    {
        if (!meters.HasValue)
            return "0 m";

        int UnsignedMeters = Math.Abs(meters.Value);

        cultureInfo ??= System.Globalization.CultureInfo.InvariantCulture;

        return UnsignedMeters switch
        {
            >= 10_000 => $"{(UnsignedMeters / 1_000).ToString("0", cultureInfo)} km",
            > 997 => $"{(UnsignedMeters / 1_000).ToString("0.0", cultureInfo)} km",
            < 1 => $"{UnsignedMeters * 100:0} cm",
            _ => $"{UnsignedMeters:0} m",
        };
    }
    public static string ToReadableDistance(long? meters, System.Globalization.CultureInfo? cultureInfo = null)
    {
        if (!meters.HasValue)
            return "0 m";

        long UnsignedMeters = Math.Abs(meters.Value);

        cultureInfo ??= System.Globalization.CultureInfo.InvariantCulture;

        return UnsignedMeters switch
        {
            >= 10_000 => $"{(UnsignedMeters / 1_000).ToString("0", cultureInfo)} km",
            > 997 => $"{(UnsignedMeters / 1_000).ToString("0.0", cultureInfo)} km",
            < 1 => $"{UnsignedMeters * 100:0} cm",
            _ => $"{UnsignedMeters:0} m",
        };
    }
}
