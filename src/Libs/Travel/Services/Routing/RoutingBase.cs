namespace Seedysoft.Libs.Travel.Services.Routing;

internal abstract class RoutingBase(Settings.RoutingApi api)
{
    protected RestSharp.RestClient RestClient { get; } = new(new Uri(api.UrlFormat).GetLeftPart(UriPartial.Authority));
    protected Settings.RoutingApi Api { get; } = api;

    /// <summary>
    /// Obtiene las rutas entre el origen y el destino especificados en el modelo de consulta.
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="dest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal abstract Task<IList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(Models.Location orig, Models.Location dest, CancellationToken cancellationToken);
    // TODO                             Return an object with more data
    internal static double[,] InvertLongitudeLatitude(double[,] matrix)
    {
        double[,] newArray = new double[matrix.GetLength(0), matrix.GetLength(1)];

        for (int row = 0; row < newArray.GetLength(0); row++)
        {
            for (int col = 0; col < newArray.GetLength(1); col++)
            {
                newArray.SetValue(matrix[row, col == 0 ? 1 : 0], [row, col]);
            }
        }

        return newArray;
    }
}
