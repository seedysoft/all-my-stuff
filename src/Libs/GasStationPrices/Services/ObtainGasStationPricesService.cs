using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class ObtainGasStationPricesService(IConfiguration configuration, ILogger<ObtainGasStationPricesService> logger)
{
    private readonly Settings.GasStationPricesSettings Settings
        = configuration.GetSection(nameof(GasStationPrices.Settings.GasStationPricesSettings)).Get<Settings.GasStationPricesSettings>()!;

    public async IAsyncEnumerable<ViewModels.GasStationModel> GetGasStationsAsync(
        ViewModels.TravelQueryModel travelQueryModel,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Obtain Gas Stations with Prices from Minetur
        Json.Minetur.Body? gasStations = null;

        try
        {
            RestRequest restRequest = new(Settings.Minetur.Uris.EstacionesTerrestres);
            RestClient restClient = new(Settings.Minetur.Uris.Base)
            {
                AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
            };
            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
            if (restResponse.IsSuccessStatusCode)
                gasStations = restResponse.Content!.FromJson<Json.Minetur.Body>();
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        if (gasStations == null)
            yield break;

        for (int i = 0; i < gasStations.EstacionesTerrestres.Length; i++)
        {
            Json.Minetur.EstacionTerrestre estacionTerrestre = gasStations.EstacionesTerrestres[i];

            ViewModels.GasStationModel? gasStationModel = travelQueryModel.IsInsideBounds(estacionTerrestre);

            if (gasStationModel != null)
                yield return gasStationModel;
        }
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
