namespace Seedysoft.Libs.GoogleApis.Models;

public class DirectionsServiceRoutes
{
    public int Index { get; init; }

    public required string Summary { get; init; }

    public required string Distance { get; init; }

    public required string Duration { get; init; }

    public required string[] Warnings { get; init; } = [];
}
