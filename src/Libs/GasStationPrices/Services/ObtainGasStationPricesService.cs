using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Services;

public sealed class ObtainGasStationPricesService(IConfiguration configuration, ILogger<ObtainGasStationPricesService> logger)
{
    private readonly Core.Settings.SettingsRoot Settings
        = configuration.GetSection(nameof(Core.Settings.SettingsRoot)).Get<Core.Settings.SettingsRoot>()!;

    public async Task<string> GetMapId(CancellationToken cancellationToken)
        => await Task.FromResult(Settings.GoogleMapsPlatform.Maps.MapId);

    public async Task<IEnumerable<string>> FindPlacesAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestRequest restRequest = BuildFindPlacesRequest(textToFind);
            RestClient restClient = new(Settings.GoogleMapsPlatform.PlacesApi.UriFormat);
            Core.Json.Google.Places.Response.Body? body = null;
#if DEBUG
            RestResponse restResponse = await restClient.ExecutePostAsync(restRequest, cancellationToken);
            if (restResponse.IsSuccessStatusCode)
                body = restResponse.Content!.FromJson<Core.Json.Google.Places.Response.Body>();
#else
            RestResponse<Core.Json.Google.Places.Response.Body> restResponse =
                (await restClient.ExecutePostAsync<Core.Json.Google.Places.Response.Body>(restRequest, cancellationToken)).ThrowIfError();
            body = restResponse.Data!;
#endif
            if (body == null)
                return [];

            IEnumerable<string> places =
                from p in body.Suggestions.Select(x => x.PlacePrediction)
                where !string.IsNullOrWhiteSpace(p.Text?.Text)
                select p.Text!.Text;

            return places.ToArray();
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];

        RestRequest BuildFindPlacesRequest(string textToFind)
        {
            RestRequest restRequest = new();
            restRequest = restRequest.AddHeader("X-Goog-Api-Key", Settings.GoogleMapsPlatform.ApiKey);

            Core.Json.Google.Places.Request.Body PlacesRequestBody = new()
            {
                Input = textToFind,
                //LocationBias = new()
                //{
                //    Rectangle = new()
                //    {
                //        High = new() { Latitude = 1.1, Longitude = 1.1, },
                //        Low = new() { Latitude = 2.2, Longitude = 2.2, },
                //    }
                //},
                //LocationRestriction = new()
                //{
                //    Circle = new() { Center = new() { Latitude = 3.3, Longitude = 3.3, }, }
                //},
                IncludedPrimaryTypes = ["geocode", "locality", "route", "street_address"],
                //IncludedRegionCodes = [""],
                //LanguageCode = "",
                //RegionCode = "",
                //Origin = new() { Latitude = 5.5, Longitude = 5.5, },
                IncludeQueryPredictions = false,
                //SessionToken = string.Empty,
            };

            return restRequest.AddJsonBody(PlacesRequestBody.ToJson(), ContentType.Json);
        }
    }

    public async IAsyncEnumerable<Core.ViewModels.GasStationModel> GetGasStationsAsync(
        Core.ViewModels.TravelQueryModel travelQueryModel,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Obtain Gas Stations with Prices from Minetur
        Core.Json.Minetur.Body? gasStations = null;

        try
        {
            RestRequest restRequest = new(Settings.Minetur.Uris.EstacionesTerrestres);
            RestClient restClient = new(Settings.Minetur.Uris.Base) { AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,], };
#if DEBUG
            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
            if (restResponse.IsSuccessStatusCode)
                gasStations = restResponse.Content!.FromJson<Core.Json.Minetur.Body>();
#else
            RestResponse<Core.Json.Minetur.Body> restResponse =
                (await restClient.ExecutePostAsync<Core.Json.Minetur.Body>(restRequest, cancellationToken)).ThrowIfError();
            gasStations = restResponse.Data!;
#endif
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        if (gasStations == null)
            yield break;

        for (int i = 0; i < gasStations.EstacionesTerrestres.Length; i++)
        {
            Core.Json.Minetur.EstacionTerrestre estacionTerrestre = gasStations.EstacionesTerrestres[i];

            Core.ViewModels.GasStationModel? gasStationModel = travelQueryModel.IsInsideBounds(estacionTerrestre);

            // TODO     Filtrar por los productos seleccionados.
            // TODO             ¿Cómo "mapear" ProductosPetroliferos con las propiedades? ¿Switch?
            if (gasStationModel != null)
                yield return gasStationModel;
        }
    }

    private static IEnumerable<Core.Json.Minetur.ProductoPetrolifero>? Res;
    public async Task<IEnumerable<Core.Json.Minetur.ProductoPetrolifero>> GetPetroleumProductsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            if (Res == null)
            {
                RestRequest restRequest = new(string.Format(Settings.Minetur.Uris.ListadosBase, "ProductosPetroliferos"));
                RestClient restClient = new(Settings.Minetur.Uris.Base) { AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,], };
#if DEBUG
                RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessStatusCode)
                    Res = restResponse.Content!.FromJson<IEnumerable<Core.Json.Minetur.ProductoPetrolifero>>();
#else
                RestResponse<IEnumerable<Core.Json.Minetur.ProductoPetrolifero>> restResponse =
                    (await restClient.ExecuteGetAsync<IEnumerable<Core.Json.Minetur.ProductoPetrolifero>>(restRequest, cancellationToken)).ThrowIfError();
                Res = restResponse.Data!;
#endif
            }

            return Res ?? [];
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }
}
