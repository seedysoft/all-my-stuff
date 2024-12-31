namespace Seedysoft.Libs.Core.Extensions;

public static class DecimalExtensions
{
    public static decimal? ParseWithNumberFormatInfoES(this string value)
        => decimal.TryParse(value, Constants.Globalization.NumberFormatInfoES, out decimal dump) ? dump : null;
}
