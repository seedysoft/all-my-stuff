namespace Seedysoft.Libs.Geography.Services.Routers;

internal abstract class RouterBase(Settings.Api api)
{
    protected RestSharp.RestClient RestClient { get; } = new(new Uri(api.UrlFormat).GetLeftPart(UriPartial.Authority));
    protected Settings.Api Api { get; } = api;

    internal abstract Task<IList<(string NombreRuta, double[][] Coordenadas)>> GetRoutesAsync(ViewModels.TravelQueryModel model, CancellationToken cancellationToken);

    internal static double[][] InvertCoordinates(double[][] matrix) => [.. matrix.Select(static (row, i) => new[] { row[1], row[0] })];
}
