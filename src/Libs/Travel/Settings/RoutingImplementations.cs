namespace Seedysoft.Libs.Travel.Settings;

public readonly record struct RoutingImplementations
{
    //public const string GoogleRoutes = "GoogleRoutes";

    //public const string MapboxDirections = "MapboxDirections";

    public const string OpenSourceRoutingMachine = "OSRM";

    public const string Valhalla = nameof(Valhalla);

    //public const string Here = "https://docs.here.com/routing/docs/routing-v8-get-started";
}
