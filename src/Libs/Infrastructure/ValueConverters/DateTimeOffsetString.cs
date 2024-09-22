namespace Seedysoft.Libs.Infrastructure.ValueConverters;

public static class DateTimeOffsetString
{
    public const string DateTimeOffsetToStringFormat = "yyyy-MM-dd HH:mm:sszzz";

    public static Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, string> DateTimeOffsetStringValueConverter
    {
        get
        {
            return new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, string>(
                convertToProviderExpression: static dateTimeOffset => dateTimeOffset.ToString(DateTimeOffsetToStringFormat, Core.Constants.Globalization.CultureInfoES),
                convertFromProviderExpression: static text => DateTimeOffset.ParseExact(text, DateTimeOffsetToStringFormat, Core.Constants.Globalization.CultureInfoES));
        }
    }
    public static Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset?, string?> NullableDateTimeOffsetStringValueConverter
    {
        get
        {
            return new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset?, string?>(
                convertToProviderExpression: static dateTimeOffset => dateTimeOffset.HasValue ? dateTimeOffset.Value.ToString(DateTimeOffsetToStringFormat, Core.Constants.Globalization.CultureInfoES) : default,
                convertFromProviderExpression: static text => string.IsNullOrWhiteSpace(text) ? default : DateTimeOffset.ParseExact(text, DateTimeOffsetToStringFormat, Core.Constants.Globalization.CultureInfoES));
        }
    }
}
