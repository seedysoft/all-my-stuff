namespace Seedysoft.Libs.Core.Constants;

public readonly record struct Globalization
{
    public static readonly System.Globalization.CultureInfo CultureInfoES = System.Globalization.CultureInfo.GetCultureInfo("es");

    public static readonly System.Globalization.DateTimeFormatInfo DateTimeFormatInfoES = CultureInfoES.DateTimeFormat;

    public static readonly System.Globalization.NumberFormatInfo NumberFormatInfoES = CultureInfoES.NumberFormat;

    public static readonly System.Globalization.CultureInfo CultureInfoInvariant = System.Globalization.CultureInfo.InvariantCulture;

    public static readonly System.Globalization.NumberFormatInfo NumberFormatInfoInvariant = CultureInfoInvariant.NumberFormat;
}
