using Seedysoft.Libs.GoogleApis.Models.Directions.Response;

namespace Seedysoft.Libs.GoogleApis.Models;

public class DirectionsServiceRoutes
{
    public int Index { get; init; }

    public required string Summary { get; init; }

    public required string Distance { get; init; }

    public required string Duration { get; init; }

    public required IEnumerable<string> Warnings { get; init; } = [];

    public required DirectionsRoute Route { get; init; }
}
