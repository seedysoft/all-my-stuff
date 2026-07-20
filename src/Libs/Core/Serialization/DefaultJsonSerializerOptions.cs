namespace Seedysoft.Libs.Core.Serialization;

public static class DefaultJsonSerializerOptions
{
    public static System.Text.Json.JsonSerializerOptions DefaultsReadOnly
    {
        get
        {
            System.Text.Json.JsonSerializerOptions? jsonSerializerOptions = GetSettings();

            return jsonSerializerOptions;
        }
    }
    public static System.Text.Json.JsonSerializerOptions GetSettings() => new(System.Text.Json.JsonSerializerDefaults.Web)
    {
        //Converters =
        //{
        //    new JsonStringEnumConverter(),
        //    new DateOnlyConverter(),
        //    new TimeOnlyConverter(),
        //    IsoDateTimeOffsetConverter.Singleton
        //},
        AllowDuplicateProperties = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        RespectNullableAnnotations = true,
        WriteIndented = false,
    };
}
