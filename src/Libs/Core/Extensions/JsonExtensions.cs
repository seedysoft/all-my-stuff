namespace Seedysoft.Libs.Core.Extensions;

public static class JsonExtensions
{
    public static T FromJson<T>(this string text) =>
        System.Text.Json.JsonSerializer.Deserialize<T>(
            json: text,
            options: Serialization.DefaultJsonSerializerOptions.DefaultsReadOnly)
        ?? throw new InvalidOperationException();

    public static string ToJson<T>(
        this T self,
        bool allowReadOnlyFields = false,
        bool allowReadOnlyProperties = false)
    {
        System.Text.Json.JsonSerializerOptions options;
        if (allowReadOnlyFields || allowReadOnlyProperties)
        {
            options = Serialization.DefaultJsonSerializerOptions.GetSettings();
            options.IgnoreReadOnlyProperties = !allowReadOnlyProperties;
            options.IgnoreReadOnlyProperties = !allowReadOnlyProperties;
        }
        else
        {
            options = Serialization.DefaultJsonSerializerOptions.DefaultsReadOnly;
        }

        return System.Text.Json.JsonSerializer.Serialize(self, options);
    }
}
