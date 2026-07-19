//namespace Seedysoft.Libs.Core.Serialization;

//public class IsoDateTimeOffsetConverter : System.Text.Json.Serialization.JsonConverter<DateTimeOffset>
//{
//    public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

//    private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

//    public System.Globalization.DateTimeStyles DateTimeStyles { get; set; } = System.Globalization.DateTimeStyles.RoundtripKind;

//    public string? DateTimeFormat
//    {
//        get => field ?? string.Empty;
//        set => field = string.IsNullOrEmpty(value) ? null : value;
//    }

//    public System.Globalization.CultureInfo Culture
//    {
//        get => field ?? System.Globalization.CultureInfo.CurrentCulture;
//        set;
//    }

//    public override void Write(System.Text.Json.Utf8JsonWriter writer, DateTimeOffset value, System.Text.Json.JsonSerializerOptions options)
//    {
//        if ((DateTimeStyles & System.Globalization.DateTimeStyles.AdjustToUniversal) == System.Globalization.DateTimeStyles.AdjustToUniversal ||
//            (DateTimeStyles & System.Globalization.DateTimeStyles.AssumeUniversal) == System.Globalization.DateTimeStyles.AssumeUniversal)
//        {
//            value = value.ToUniversalTime();
//        }

//        string text = value.ToString(DateTimeFormat ?? DefaultDateTimeFormat, Culture);

//        writer.WriteStringValue(text);
//    }

//    public override DateTimeOffset Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//    {
//        string? dateText = reader.GetString();

//        return string.IsNullOrEmpty(dateText)
//            ? default
//            : string.IsNullOrEmpty(DateTimeFormat)
//                ? DateTimeOffset.Parse(dateText, Culture, DateTimeStyles)
//                : DateTimeOffset.ParseExact(dateText, DateTimeFormat, Culture, DateTimeStyles);
//    }

//    //public static readonly IsoDateTimeOffsetConverter Singleton = new();
//}
