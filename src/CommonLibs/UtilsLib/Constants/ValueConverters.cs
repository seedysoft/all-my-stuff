namespace Seedysoft.UtilsLib.Constants;

public static class ValueConverters
{
    public const string DateTimeOffsetToStringFormat = "yyyy-MM-dd HH:mm:sszzz";

    public static Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, string> DateTimeOffsetStringValueConverter
    {
        get
        {
            return new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, string>(
                convertToProviderExpression: dateTimeOffset => dateTimeOffset.ToString(DateTimeOffsetToStringFormat, Formats.ESCultureInfo),
                convertFromProviderExpression: text => DateTimeOffset.ParseExact(text, DateTimeOffsetToStringFormat, Formats.ESCultureInfo));
        }
    }
}
