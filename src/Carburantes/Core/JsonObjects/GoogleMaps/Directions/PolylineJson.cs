﻿using Seedysoft.Carburantes.Core.Extensions;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Seedysoft.Carburantes.Core.JsonObjects.GoogleMaps.Directions;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public record PolylineJson
{
    [JsonPropertyName("points")]
    public string Points { get; set; } = default!;

    public LocationJson[] Locations()
    {
        return (LocationJson[])(string.IsNullOrEmpty(Points)
            ? Enumerable.Empty<LocationJson>()
            : GoogleHelper.Decode(Points).ToArray());
    }

    private string GetDebuggerDisplay() => $"{Points}";
}
