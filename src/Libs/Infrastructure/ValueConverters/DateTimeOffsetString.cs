namespace Seedysoft.Libs.Infrastructure.ValueConverters;

public static class DateTimeOffsetString
{
    public const string DateTimeOffsetToStringFormat = "yyyy-MM-dd HH:mm:sszzz";

    public static Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, string> DateTimeOffsetStringValueConverter
    {
        get
        {
            return new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, string>(
                convertToProviderExpression: dateTimeOffset => dateTimeOffset.ToString(DateTimeOffsetToStringFormat, Utils.Constants.Formats.ESCultureInfo),
                convertFromProviderExpression: text => DateTimeOffset.ParseExact(text, DateTimeOffsetToStringFormat, Utils.Constants.Formats.ESCultureInfo));
        }
    }
    public static Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset?, string?> NullableDateTimeOffsetStringValueConverter
    {
        get
        {
            return new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset?, string?>(
                convertToProviderExpression: dateTimeOffset => dateTimeOffset.HasValue ? dateTimeOffset.Value.ToString(DateTimeOffsetToStringFormat, Utils.Constants.Formats.ESCultureInfo) : default,
                convertFromProviderExpression: text => string.IsNullOrWhiteSpace(text) ? default : DateTimeOffset.ParseExact(text, DateTimeOffsetToStringFormat, Utils.Constants.Formats.ESCultureInfo));
        }
    }
}
