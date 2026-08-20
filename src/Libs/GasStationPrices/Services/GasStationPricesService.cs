using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.GasStationPrices.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class GasStationPricesService
{
    public Settings.GasStationPricesSettings GasStationPricesSettings { get; init; }

    private readonly ILogger<GasStationPricesService> Logger;

    private static Models.Minetur.Body? MineturResponse;
    private bool isLoading;

    public GasStationPricesService(IServiceProvider serviceProvider)
    {
        GasStationPricesSettings = serviceProvider.GetRequiredService<IConfiguration>()
            .GetSection(nameof(Settings.GasStationPricesSettings))
            .Get<Settings.GasStationPricesSettings>()!;

        Logger = serviceProvider.GetRequiredService<ILogger<GasStationPricesService>>();

        _ = Task.Run(async () => await LoadGasStationsAsync(CancellationToken.None));
    }

    public async Task<IReadOnlyList<ViewModels.GasStationModel>> GetNearGasStationsAsync(
        Travel.Models.Bounds bounds,
        int maxDistanceInKm,
        CancellationToken cancellationToken)
    {
        return await LoadGasStationsAsync(cancellationToken)
            ? [.. MineturResponse?.EstacionesTerrestres.Where(x => bounds.IsInside(x.LatLng)).Select(x => x.ToGasStationModel()) ?? []]
            : [];
    }

    /// <summary>
    /// Obtain Gas Stations with Prices from Minetur
    /// </summary>
    /// <returns><c>true</c> if MineturResponse is not null or <c>false</c> otherwise.</returns>
    private async Task<bool> LoadGasStationsAsync(CancellationToken cancellationToken)
    {
        if (!isLoading && (MineturResponse == null || MineturResponse?.DateTimeOffset < DateTimeOffset.Now.AddMinutes(-35)))
        {
            try
            {
                isLoading = true;

                var sw = System.Diagnostics.Stopwatch.StartNew();

                RestRequest restRequest = new(GasStationPricesSettings.Minetur.Urls.EstacionesTerrestres)
                {
                    Version = System.Net.HttpVersion.Version20,
                };
                RestClient restClient = new(GasStationPricesSettings.Minetur.Urls.Base)
                {
                    AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
                };
                //restClient.Options.RemoteCertificateValidationCallback?. = () => { };
                System.Diagnostics.Debugger.Break();

                RestClientOptions options = new(GasStationPricesSettings.Minetur.Urls.Base)
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                };
                RestClient client = new(options);
                RestResponse response = await client.GetAsync(restRequest, cancellationToken);

                RestResponse restResponse = await restClient.GetAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessStatusCode)
                    MineturResponse = restResponse.Content!.FromJson<Models.Minetur.Body>();

                sw.Stop();
                if (Logger.IsEnabled(LogLevel.Information))
                    Logger.LogInformation("Loaded gas stations in {Elapsed} secs.", sw.Elapsed.ToString(@"s\.fff"));
            }
            catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
            finally { isLoading = false; }
        }

        return MineturResponse != null;
    }
}
