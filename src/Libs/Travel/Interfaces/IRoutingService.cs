namespace Seedysoft.Libs.Travel.Interfaces;

public interface IRoutingService
{
    Task<IReadOnlyList<(string NombreRuta, double[,] Coordenadas)>> RouteAsync(
        Core.Models.Location orig,
        Core.Models.Location dest,
        CancellationToken cancellationToken);
}
