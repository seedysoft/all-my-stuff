using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Services;

// TODO                 Remove Obtain from name
public sealed class ObtainGasStationPricesService(
    IServiceProvider serviceProvider,
    GoogleApis.Services.RoutesService googleApisServicesRoutesService)
{
    private readonly Settings.GasStationPricesSettings GasStationPricesSettings = serviceProvider.GetRequiredService<IConfiguration>()
        .GetSection(nameof(Settings.GasStationPricesSettings)).Get<Settings.GasStationPricesSettings>()!;
    private readonly ILogger<ObtainGasStationPricesService> Logger = serviceProvider.GetRequiredService<ILogger<ObtainGasStationPricesService>>();

    public async IAsyncEnumerable<ViewModels.GasStationModel> GetGasStationsAsync(
        ViewModels.TravelQueryModel travelQueryModel,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Obtain Gas Stations with Prices from Minetur
        Models.Minetur.Body? MineturResponse = null;

        try
        {
            RestRequest restRequest = new(GasStationPricesSettings.Minetur.Uris.EstacionesTerrestres);
            RestClient restClient = new(GasStationPricesSettings.Minetur.Uris.Base)
            {
                AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
            };
            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
            if (restResponse.IsSuccessStatusCode)
                MineturResponse = restResponse.Content!.FromJson<Models.Minetur.Body>();
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

        if (MineturResponse == null)
            yield break;

        List<GoogleApis.Models.Shared.LatLngLiteral> RoutePoints = await
            googleApisServicesRoutesService.GetRoutesAsync(travelQueryModel.Origin, travelQueryModel.Destination, cancellationToken);

        if (RoutePoints.Count < 1)
            throw new ApplicationException($"{nameof(GoogleApis.Services.RoutesService.GetRoutesAsync)} does not finds route points");

        GoogleApis.Models.Shared.LatLngBoundsLiteral boundsLiteral = new()
        {
            North = RoutePoints.Select(x => x.Lat).Max(),
            South = RoutePoints.Select(x => x.Lat).Min(),
            East = RoutePoints.Select(x => x.Lng).Max(),
            West = RoutePoints.Select(x => x.Lng).Min(),
        };

        IEnumerable<Models.Minetur.EstacionTerrestre> NearStations = MineturResponse.EstacionesTerrestres
            .AsParallel()
            .Where(x => x.IsInsideBounds(boundsLiteral) && RoutePoints.Any(y => x.IsNear(y, travelQueryModel.MaxDistanceInKm)));

        foreach (Models.Minetur.EstacionTerrestre et in NearStations)
            yield return ViewModels.GasStationModel.Map(et);
    }

    //private static IEnumerable<Core.Json.Minetur.ProductoPetrolifero>? Res;
    //public async Task<IEnumerable<Core.Json.Minetur.ProductoPetrolifero>> GetPetroleumProductsAsync(
    //    CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        if (Res == null)
    //        {
    //            RestRequest restRequest = new(string.Format(Settings.Minetur.Uris.ListadosBase, "ProductosPetroliferos"));
    //            RestClient restClient = new(Settings.Minetur.Uris.Base) { AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,], };
    //            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
    //            if (restResponse.IsSuccessStatusCode)
    //                Res = restResponse.Content!.FromJson<IEnumerable<Core.Json.Minetur.ProductoPetrolifero>>();
    //        }

    //        return Res ?? [];
    //    }
    //    catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

    //    return [];
    //}
}
