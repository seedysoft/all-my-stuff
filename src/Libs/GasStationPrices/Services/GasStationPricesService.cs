using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.GasStationPrices.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class GasStationPricesService(IServiceProvider serviceProvider)
{
    public Settings.GasStationPricesSettings GasStationPricesSettings { get; init; } = serviceProvider.GetRequiredService<IConfiguration>()
        .GetSection(nameof(Settings.GasStationPricesSettings))
        .Get<Settings.GasStationPricesSettings>()!;
    private readonly ILogger<GasStationPricesService> Logger = serviceProvider.GetRequiredService<ILogger<GasStationPricesService>>();

    public async IAsyncEnumerable<ViewModels.GasStationModel> GetNearGasStationsAsync(
        string encodedPolyline,
        int maxDistanceInKm)
    {
        // Obtain Gas Stations with Prices from Minetur
        Models.Minetur.Body? MineturResponse = null;
        try
        {
            RestRequest restRequest = new(GasStationPricesSettings.Minetur.Urls.EstacionesTerrestres);
            RestClient restClient = new(GasStationPricesSettings.Minetur.Urls.Base)
            {
                AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
            };
            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest);
            if (restResponse.IsSuccessStatusCode)
                MineturResponse = restResponse.Content!.FromJson<Models.Minetur.Body>();
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

        IEnumerable<GoogleApis.Models.Shared.LatLngLiteral> RoutePoints = GoogleApis.Helpers.GooglePolyline.Decode(encodedPolyline);

        for (int i = 0; i < MineturResponse?.EstacionesTerrestres.Length; i++)
        {
            // TODO                 TEST SPEED
            Models.Minetur.EstacionTerrestre Estacion = MineturResponse?.EstacionesTerrestres[i]!;
            GoogleApis.Models.Shared.LatLngLiteral from = Estacion.LatLng;
            //if (RoutePoints.Any(x => Estacion.IsNear(x, maxDistanceInKm)))
            if (RoutePoints.Any(x => GoogleApis.Helpers.GeometricHelper.GetDistance(from, x) < maxDistanceInKm))
                yield return Estacion.ToGasStationModel();
        }
    }
}
