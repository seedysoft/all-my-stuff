using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.GasStationPrices.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class GasStationPricesService
{
    public Settings.GasStationPricesSettings GasStationPricesSettings { get; init; }

    private readonly IHttpClientFactory httpClientFactory;

    private readonly ILogger<GasStationPricesService> Logger;

    private static readonly AsyncLocal<Models.Minetur.Body?> MineturResponse = new();

    public GasStationPricesService(IServiceProvider serviceProvider)
    {
        GasStationPricesSettings = serviceProvider.GetRequiredService<IConfiguration>()
            .GetSection(nameof(Settings.GasStationPricesSettings))
            .Get<Settings.GasStationPricesSettings>()!;

        httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        Logger = serviceProvider.GetRequiredService<ILogger<GasStationPricesService>>();

        _ = Task.Run(async () =>
        {
            while (!await LoadGasStationsAsync(CancellationToken.None))
                await Task.Delay(TimeSpan.FromSeconds(10));
        });
    }

    public async Task<ViewModels.GasStationModel?> GetGasStationAsync(
        Travel.Models.Location latLng,
        CancellationToken cancellationToken)
    {
        return await LoadGasStationsAsync(cancellationToken) && MineturResponse.Value != null
            ? MineturResponse.Value.Value.EstacionesTerrestres.FirstOrDefault(x => latLng.Equals(x.LatLng)).ToGasStationModel()
            : null;
    }

    public async Task<IReadOnlyList<ViewModels.GasStationModel>> GetNearGasStationsAsync(
        Travel.Models.Bounds bounds,
        CancellationToken cancellationToken)
    {
        return await LoadGasStationsAsync(cancellationToken) && MineturResponse.Value != null
            ? [.. MineturResponse.Value.Value.EstacionesTerrestres.Where(x => bounds.IsInside(x.LatLng)).Select(x => x.ToGasStationModel()!) ?? []]
            : [];
    }

    /// <summary>
    /// Obtain Gas Stations with Prices from Minetur
    /// </summary>
    /// <returns><c>true</c> if MineturResponse is not null or <c>false</c> otherwise.</returns>
    private async Task<bool> LoadGasStationsAsync(CancellationToken cancellationToken)
    {
        if (MineturResponse.Value == null || MineturResponse.Value.Value.DateTimeOffset < DateTimeOffset.Now.AddMinutes(-35))
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                //Uri requestUri = GasStationPricesSettings.Minetur.Urls.GetUri();

                using HttpClient httpClient = httpClientFactory.CreateClient(nameof(GasStationPrices));
                MineturResponse.Value = await httpClient.GetFromJsonAsync<Models.Minetur.Body>(GasStationPricesSettings.Minetur.Urls.EstacionesTerrestresEndPoint, cancellationToken);

                sw.Stop();
                if (Logger.IsEnabled(LogLevel.Information))
                    Logger.LogInformation("Loaded gas stations in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));
            }
            catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { MineturResponse.Value = null; }
        }

        return MineturResponse?.Value != null;
    }
}
