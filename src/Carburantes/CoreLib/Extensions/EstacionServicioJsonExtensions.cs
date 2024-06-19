using Seedysoft.Carburantes.CoreLib.JsonObjects.Minetur;

namespace Seedysoft.Carburantes.CoreLib.Extensions;

public static class EstacionServicioJsonExtensions
{
    public static NetTopologySuite.Geometries.Coordinate ToCoordinate(this EstacionesServicioJson estacionServicioJson) =>
        new(double.Parse(estacionServicioJson.LongitudWgs84!), double.Parse(estacionServicioJson.Latitud!));
}
