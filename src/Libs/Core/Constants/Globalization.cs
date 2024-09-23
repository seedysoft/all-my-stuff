namespace Seedysoft.Libs.Core.Constants;

public readonly record struct Globalization
{
    public static readonly System.Globalization.CultureInfo CultureInfoES = System.Globalization.CultureInfo.GetCultureInfo("es");

    public static readonly System.Globalization.NumberFormatInfo NumberFormatInfoES = CultureInfoES.NumberFormat;
}
