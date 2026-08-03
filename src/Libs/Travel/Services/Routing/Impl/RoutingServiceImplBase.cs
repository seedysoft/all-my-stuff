namespace Seedysoft.Libs.Travel.Services.Routing.Impl;

/// <summary>
/// Base class for routing service implementations.
/// Provides common routing-specific functionality and delegates HTTP client management to the generic base.
/// </summary>
internal abstract class RoutingServiceImplBase(IHttpClientFactory httpClientFactory, Settings.RoutingServiceApi routingApi)
    : HttpServiceImplBase<Settings.RoutingServiceApi>(httpClientFactory, routingApi)
{
    /// <summary>
    /// Obtains the routes between the specified origin and destination locations.
    /// </summary>
    /// <param name="orig">The origin location coordinates.</param>
    /// <param name="dest">The destination location coordinates.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of tuples containing route names and their corresponding coordinates.</returns>
    internal abstract Task<IReadOnlyList<(string NombreRuta, double[,] Coordenadas)>> GetRoutesAsync(
        Models.Location orig
        , Models.Location dest
        , CancellationToken cancellationToken);

    /// <summary>
    /// Inverts longitude and latitude values in a 2D coordinate matrix.
    /// Converts from [lng, lat] format to [lat, lng] format.
    /// </summary>
    /// <param name="matrix">The matrix with coordinates in [lng, lat] format.</param>
    /// <returns>A new matrix with coordinates in [lat, lng] format.</returns>
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
