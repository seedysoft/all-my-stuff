﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class ObtainGasStationPricesService(
    IConfiguration configuration,
    GoogleApis.Services.RoutesService googleApisServicesRoutesService,
    ILogger<ObtainGasStationPricesService> logger)
{
    private readonly Settings.GasStationPricesSettings GasStationPricesSettings
        = configuration.GetSection(nameof(Settings.GasStationPricesSettings)).Get<Settings.GasStationPricesSettings>()!;

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
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

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
