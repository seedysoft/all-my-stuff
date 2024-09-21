using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seedysoft.Libs.Utils.Extensions;

public static class JsonExtensions
{
    public static T FromJson<T>(this string json) => JsonSerializer.Deserialize<T>(json, Converter.Settings) ?? throw new InvalidOperationException();

    public static string ToJson<T>(this T self) => JsonSerializer.Serialize(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.Web)
    {
        //Converters =
        //{
        //    new JsonStringEnumConverter(),
        //    new DateOnlyConverter(),
        //    new TimeOnlyConverter(),
        //    IsoDateTimeOffsetConverter.Singleton
        //},
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        PropertyNameCaseInsensitive = true,
    };
}

//public class DateOnlyConverter(string? serializationFormat) : JsonConverter<DateOnly>
//{
//    private readonly string serializationFormat = serializationFormat ?? "yyyy-MM-dd";

//    public DateOnlyConverter() : this(null) { }

//    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        => DateOnly.Parse(reader.GetString()!);

//    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
//        => writer.WriteStringValue(value.ToString(serializationFormat));
//}

//public class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
//{
//    public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

//    private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
//    private string? _dateTimeFormat;
//    private System.Globalization.CultureInfo? _culture;

//    public System.Globalization.DateTimeStyles DateTimeStyles { get; set; } = System.Globalization.DateTimeStyles.RoundtripKind;

//    public string? DateTimeFormat
//    {
//        get => _dateTimeFormat ?? string.Empty;
//        set => _dateTimeFormat = string.IsNullOrEmpty(value) ? null : value;
//    }

//    public System.Globalization.CultureInfo Culture
//    {
//        get => _culture ?? System.Globalization.CultureInfo.CurrentCulture;
//        set => _culture = value;
//    }

//    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
//    {
//        if ((DateTimeStyles & System.Globalization.DateTimeStyles.AdjustToUniversal) == System.Globalization.DateTimeStyles.AdjustToUniversal ||
//            (DateTimeStyles & System.Globalization.DateTimeStyles.AssumeUniversal) == System.Globalization.DateTimeStyles.AssumeUniversal)
//        {
//            value = value.ToUniversalTime();
//        }

//        string text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

//        writer.WriteStringValue(text);
//    }

//    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        string? dateText = reader.GetString();

//        return string.IsNullOrEmpty(dateText)
//            ? default
//            : string.IsNullOrEmpty(_dateTimeFormat)
//                ? DateTimeOffset.Parse(dateText, Culture, DateTimeStyles)
//                : DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, DateTimeStyles);
//    }

//    public static readonly IsoDateTimeOffsetConverter Singleton = new();
//}

public class ParseStringConverter : JsonConverter<long>
{
    public override bool CanConvert(Type t) => t == typeof(long);

    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => long.TryParse(reader.GetString(), out long l) ? l : throw new Exception("Cannot unmarshal type long");

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value.ToString(), options);

    public static readonly ParseStringConverter Singleton = new();
}

//public class TimeOnlyConverter(string? serializationFormat) : JsonConverter<TimeOnly>
//{
//    private readonly string serializationFormat = serializationFormat ?? "HH:mm:ss.fff";

//    public TimeOnlyConverter() : this(null) { }

//    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        => TimeOnly.Parse(reader.GetString()!);

//    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
//        => writer.WriteStringValue(value.ToString(serializationFormat));
//}
