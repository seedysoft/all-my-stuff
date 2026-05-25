using Seedysoft.Libs.Core.Json;
using System.Text.Json;

namespace Seedysoft.Libs.Core.Extensions;

public static class JsonExtensions
{
    public static T FromJson<T>(this string json)
        => JsonSerializer.Deserialize<T>(json, Converter.Settings) ?? throw new InvalidOperationException();

    public static string ToJson<T>(this T self)
        => JsonSerializer.Serialize(self, Converter.Settings);
}
