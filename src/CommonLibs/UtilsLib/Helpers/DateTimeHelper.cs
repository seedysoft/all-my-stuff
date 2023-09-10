namespace Seedysoft.UtilsLib.Helpers;

public static class DateTimeHelper
{
    public static string ToLocalTime(string utcTime, string format)
    {
        return string.IsNullOrEmpty(utcTime) || !DateTime.TryParseExact(utcTime, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime DateTimeParsed)
            ? string.Empty
            : DateTimeParsed.ToLocalTime().ToString(format);
    }
}
