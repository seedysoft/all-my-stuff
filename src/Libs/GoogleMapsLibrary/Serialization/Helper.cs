using System.Text.Json;

namespace GoogleMapsLibrary.Serialization;

public static class Helper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    static Helper()
    {
        Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        Options.Converters.Add(new OneOfConverterFactory());
    }

    public static object? DeSerializeObject(JsonElement json, Type type)
    {
        object? obj = json.Deserialize(type, Options);

        return obj;
    }
    public static object? DeSerializeObject(string? json, Type type)
    {
        if (json == null)
            return default;

        object? obj = JsonSerializer.Deserialize(json, type, Options);

        return obj;
    }
    public static TObject? DeSerializeObject<TObject>(string? json)
    {
        if (json == null)
            return default;

        TObject? value = JsonSerializer.Deserialize<TObject>(json, Options);

        return value;
    }

    public static string SerializeObject(object obj)
    {
        string value = JsonSerializer.Serialize(obj, Options);

        return value;
    }
}
