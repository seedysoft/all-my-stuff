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
            RestResponse restResponse = await restClient.ExecutePostAsync(restRequest, cancellationToken);
            if (restResponse.IsSuccessStatusCode)
                body = restResponse.Content!.FromJson<Core.Json.Google.Places.Response.Body>();
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
                // Required
                Input = textToFind,
                // Optional
                IncludedPrimaryTypes = ["geocode", "locality", "route", "street_address"],
                IncludeQueryPredictions = false,
                //IncludedRegionCodes = [""],
                //LanguageCode = "",
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
                //RegionCode = "",
                //Origin = new() { Latitude = 5.5, Longitude = 5.5, },
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
            RestClient restClient = new(Settings.Minetur.Uris.Base)
            {
                AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
            };
            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
            if (restResponse.IsSuccessStatusCode)
                gasStations = restResponse.Content!.FromJson<Core.Json.Minetur.Body>();
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
                RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessStatusCode)
                    Res = restResponse.Content!.FromJson<IEnumerable<Core.Json.Minetur.ProductoPetrolifero>>();
            }

            return Res ?? [];
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }
}
