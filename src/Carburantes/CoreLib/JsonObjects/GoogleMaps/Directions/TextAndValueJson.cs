using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.CoreLib.JsonObjects.GoogleMaps.Directions;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public record TextAndValueJson
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = default!;

    [JsonPropertyName("value")]
    public int? Value { get; set; }

    private string GetDebuggerDisplay() => $"{Text}: {Value}";
}
