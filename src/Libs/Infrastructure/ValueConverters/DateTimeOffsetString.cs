using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Seedysoft.Libs.Infrastructure.ValueConverters;

public static class DatetimeoffsetString
{
    private const string DatetimeoffsetToStringFormat = "yyyy-MM-dd HH:mm:sszzz";

    public static ValueConverter<DateTimeOffset, string> DatetimeoffsetStringValueConverter
    {
        get
        {
            return new ValueConverter<DateTimeOffset, string>(
                convertToProviderExpression: static dateTimeOffset => dateTimeOffset.ToString(DatetimeoffsetToStringFormat, Core.Constants.Globalization.CultureInfoES),
                convertFromProviderExpression: static text => DateTimeOffset.ParseExact(text, DatetimeoffsetToStringFormat, Core.Constants.Globalization.CultureInfoES));
        }
    }
    public static ValueConverter<DateTimeOffset?, string?> NullableDatetimeoffsetStringValueConverter
    {
        get
        {
            return new ValueConverter<DateTimeOffset?, string?>(
                convertToProviderExpression: static dateTimeOffset => dateTimeOffset.HasValue ? dateTimeOffset.Value.ToString(DatetimeoffsetToStringFormat, Core.Constants.Globalization.CultureInfoES) : default,
                convertFromProviderExpression: static text => string.IsNullOrWhiteSpace(text) ? default : DateTimeOffset.ParseExact(text, DatetimeoffsetToStringFormat, Core.Constants.Globalization.CultureInfoES));
        }
    }
}
