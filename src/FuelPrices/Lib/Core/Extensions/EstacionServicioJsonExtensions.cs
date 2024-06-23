namespace Seedysoft.FuelPrices.Lib.Core.Extensions;

public static class EstacionServicioJsonExtensions
{
    public static NetTopologySuite.Geometries.Coordinate ToCoordinate(this JsonObjects.Minetur.EstacionesServicioJson estacionServicioJson) =>
        new(double.Parse(estacionServicioJson.LongitudWgs84!), double.Parse(estacionServicioJson.Latitud!));
}
