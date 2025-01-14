using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.GasStationPrices.Extensions;
using System.Collections.Frozen;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class GasStationPricesService(IServiceProvider serviceProvider)
{
    public Settings.GasStationPricesSettings GasStationPricesSettings { get; init; } = serviceProvider.GetRequiredService<IConfiguration>()
        .GetSection(nameof(Settings.GasStationPricesSettings))
        .Get<Settings.GasStationPricesSettings>()!;
    private readonly ILogger<GasStationPricesService> Logger = serviceProvider.GetRequiredService<ILogger<GasStationPricesService>>();

    public async Task<IReadOnlySet<ViewModels.GasStationModel>> GetNearGasStationsAsync(
        IReadOnlySet<GoogleApis.Models.Shared.LatLngLiteral> routePoints,
        int maxDistanceInKm)
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
            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest);
            if (restResponse.IsSuccessStatusCode)
                MineturResponse = restResponse.Content!.FromJson<Models.Minetur.Body>();
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

        var NearStations = MineturResponse?.EstacionesTerrestres
            .AsParallel()
            .Where(x => routePoints.Any(y => x.IsNear(y, maxDistanceInKm)))
            .Select(x => x.ToGasStationModel())
            .ToFrozenSet();

        return NearStations ?? FrozenSet<ViewModels.GasStationModel>.Empty;
    }

    //public async IAsyncEnumerable<ViewModels.GasStationModel> GetGasStationsAsync(
    //    IEnumerable<GoogleApis.Models.Shared.LatLngLiteral> routePoints,
    //    [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    //{
    //    // Obtain Gas Stations with Prices from Minetur
    //    Models.Minetur.Body? MineturResponse = null;

    //    try
    //    {
    //        RestRequest restRequest = new(GasStationPricesSettings.Minetur.Uris.EstacionesTerrestres);
    //        RestClient restClient = new(GasStationPricesSettings.Minetur.Uris.Base)
    //        {
    //            AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
    //        };
    //        RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
    //        if (restResponse.IsSuccessStatusCode)
    //            MineturResponse = restResponse.Content!.FromJson<Models.Minetur.Body>();
    //    }
    //    catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

    //    if (MineturResponse == null)
    //        yield break;

    //    if (!routePoints.Any())
    //        throw new ApplicationException($"{nameof(GoogleApis.Services.RoutesService.GetRoutesAsync)} does not finds route points");

    //    IEnumerable<Models.Minetur.EstacionTerrestre> NearStations = MineturResponse.EstacionesTerrestres
    //        .AsParallel()
    //        .Where(x => routePoints.Any(y => x.IsNear(y, travelQueryModel.MaxDistanceInKm)));

    //    foreach (Models.Minetur.EstacionTerrestre et in NearStations)
    //        yield return ViewModels.GasStationModel.Map(et);
    //}

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
