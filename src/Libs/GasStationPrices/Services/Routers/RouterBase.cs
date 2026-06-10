using Seedysoft.Libs.GasStationPrices.Settings;
using Seedysoft.Libs.GasStationPrices.ViewModels;

namespace Seedysoft.Libs.GasStationPrices.Services.Routers;

internal abstract class RouterBase(Api api)
{
    protected RestSharp.RestClient RestClient { get; } = new(new Uri(api.UrlFormat).GetLeftPart(UriPartial.Authority));
    protected Api Api { get; } = api;

    /// <summary>
    /// Obtiene las rutas entre el origen y el destino especificados en el modelo de consulta.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal abstract Task<IList<(string NombreRuta, double[][] Coordenadas)>> GetRoutesAsync(TravelQueryModel model, CancellationToken cancellationToken);

    internal static double[][] InvertLongitudeLatitude(double[][] matrix) => [.. matrix.Select(static (row, i) => new[] { row[1], row[0] })];
}
