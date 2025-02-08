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

    private static Models.Minetur.Body? MineturResponse = null;

    public async IAsyncEnumerable<ViewModels.GasStationModel> GetNearGasStationsAsync(
        string encodedPolyline,
        int maxDistanceInKm)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        await LoadGasStationsAsync();

        sw.Stop();
        Logger.LogInformation("Loaded gas stations in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));

        if (MineturResponse == null)
            yield break;

        sw = System.Diagnostics.Stopwatch.StartNew();

        var RoutePoints = GoogleApis.Helpers.GooglePolylineHelper.Decode(encodedPolyline).ToList();
        GoogleApis.Models.Shared.LatLngBoundsLiteral Bounds = GoogleApis.Helpers.GeometricHelper.GetBounds(RoutePoints, maxDistanceInKm);
        var StationsInsideBounds = MineturResponse.EstacionesTerrestres.Where(e => e.IsInside(Bounds)).ToFrozenSet();

        sw.Stop();
        Logger.LogInformation("Decode polyline in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));

        sw = System.Diagnostics.Stopwatch.StartNew();

        ParallelQuery<ViewModels.GasStationModel> gasStationsNear = StationsInsideBounds
            .AsParallel()
            //.Where(x => RoutePoints.Any(y => GoogleApis.Helpers.GeometricHelper.Haversine.Distance(x.Lat, x.Lng, y.Lat, y.Lng) < maxDistanceInKm))
            .Where(es => RoutePoints.Any(rp => GoogleApis.Helpers.GeometricHelper.DistanceHaversineInKilometers(es.Lat, es.Lng, rp.Lat, rp.Lng) < maxDistanceInKm))
            .Select(x => x.ToGasStationModel());

        foreach (ViewModels.GasStationModel item in gasStationsNear)
            yield return item;

        //for (int i = 0; i < StationsInsideBounds.Count; i++)
        //{
        //    // TODO                 TEST SPEED
        //    Models.Minetur.EstacionTerrestre Estacion = MineturResponse?.EstacionesTerrestres[i]!;
        //    //if (RoutePoints.Any(x => Estacion.IsNear(x, maxDistanceInKm)))
        //    //GoogleApis.Models.Shared.LatLngLiteral from = Estacion.LatLng;
        //    //if (RoutePoints.Any(x => GoogleApis.Helpers.GeometricHelper.GetDistance(from, x) < maxDistanceInKm))
        //    if (RoutePoints.Any(x => GoogleApis.Helpers.GeometricHelper.Haversine.Distance(Estacion.Lat, Estacion.Lng, x.Lat, x.Lng) < maxDistanceInKm))
        //        yield return Estacion.ToGasStationModel();
        //}

        sw.Stop();
        Logger.LogInformation("Filtered near gas stations in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));
    }

    /// <summary>
    /// Obtain Gas Stations with Prices from Minetur
    /// </summary>
    /// <returns></returns>
    private async Task LoadGasStationsAsync()
    {
        if (MineturResponse == null || MineturResponse.DateTimeOffset < DateTimeOffset.Now.AddMinutes(-35))
        {
            try
            {
                RestRequest restRequest = new(GasStationPricesSettings.Minetur.Urls.EstacionesTerrestres);
                RestClient restClient = new(GasStationPricesSettings.Minetur.Urls.Base)
                {
                    AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
                };
                RestResponse restResponse = await restClient.GetAsync(restRequest);
                if (restResponse.IsSuccessStatusCode)
                    MineturResponse = restResponse.Content!.FromJson<Models.Minetur.Body>();
            }
            catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        }
    }
}
