namespace Seedysoft.Libs.Utils.Extensions;

public static class DecimalExtensions
{
    public static decimal? ParseWithNumberFormatInfoES(this string value)
        => decimal.TryParse(value, Core.Constants.Globalization.NumberFormatInfoES, out decimal dump) ? dump : null;
}
