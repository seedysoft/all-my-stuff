using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.GasStationPrices.Extensions;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class GasStationPricesService(IServiceProvider serviceProvider)
{
    public Settings.GasStationPricesSettings GasStationPricesSettings { get; init; } = serviceProvider.GetRequiredService<IConfiguration>()
        .GetSection(nameof(Settings.GasStationPricesSettings))
        .Get<Settings.GasStationPricesSettings>()!;

    private readonly ILogger<GasStationPricesService> Logger = serviceProvider.GetRequiredService<ILogger<GasStationPricesService>>();

    private static Models.Minetur.Body MineturResponse = default!;// new() { Fecha = "", EstacionesTerrestres = [], Nota = string.Empty, ResultadoConsulta = string.Empty };

    public async IAsyncEnumerable<ViewModels.GasStationModel> GetNearGasStationsAsync(
        string encodedPolyline,
        int maxDistanceInKm,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!await LoadGasStationsAsync(cancellationToken))
            yield break;

        var sw = System.Diagnostics.Stopwatch.StartNew();

        var RoutePoints = GoogleApis.Helpers.GooglePolylineHelper.Decode(encodedPolyline).ToFrozenSet();
        GoogleApis.Models.Shared.LatLngBoundsLiteral Bounds = GoogleApis.Helpers.GeometricHelper.GetBounds(RoutePoints, maxDistanceInKm);
        var StationsInsideBounds = MineturResponse.EstacionesTerrestres.Where(e => e.IsInside(Bounds)).ToFrozenSet();

        sw.Stop();
        Logger.LogInformation("Decode polyline in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));

        sw = System.Diagnostics.Stopwatch.StartNew();

        ParallelQuery<ViewModels.GasStationModel> gasStationsNear = StationsInsideBounds
            .AsParallel()
            .Where(es => RoutePoints.Any(rp => GoogleApis.Helpers.GeometricHelper.DistanceHaversineInKilometers(es.LatLng, rp) < maxDistanceInKm))
            .Select(x => x.ToGasStationModel());

        foreach (ViewModels.GasStationModel item in gasStationsNear)
            yield return item;

        sw.Stop();
        Logger.LogInformation("Filtered near gas stations in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));
    }

    //public async Task<ViewModels.GasStationModel> GetGasStationModelAsync(long stationId, CancellationToken cancellationToken)
    //{
    //    if (!await LoadGasStationsAsync(cancellationToken))
    //        yield break;

    //    var GasStation = MineturResponse.EstacionesTerrestres.FirstOrDefault(x => x.IdEstacionServicio == stationId )?.ToGasStationModel();

    //    if (GasStation == null)
    //        return Task.FromResult(null);    yield break;

    //    return GasStation;
    //}

    /// <summary>
    /// Obtain Gas Stations with Prices from Minetur
    /// </summary>
    /// <returns><code>true</code> if MineturResponse is not null. <code>false</code> if is null</returns>
    private async Task<bool> LoadGasStationsAsync(CancellationToken cancellationToken)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        if (MineturResponse == null || MineturResponse.DateTimeOffset < DateTimeOffset.Now.AddMinutes(-35))
        {
            try
            {
                RestRequest restRequest = new(GasStationPricesSettings.Minetur.Urls.EstacionesTerrestres);
                RestClient restClient = new(GasStationPricesSettings.Minetur.Urls.Base)
                {
                    AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
                };
                RestResponse restResponse = await restClient.GetAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessStatusCode)
                    MineturResponse = restResponse.Content!.FromJson<Models.Minetur.Body>();
            }
            catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        }

        sw.Stop();
        Logger.LogInformation("Loaded gas stations in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));

        return MineturResponse != null;
    }
}
