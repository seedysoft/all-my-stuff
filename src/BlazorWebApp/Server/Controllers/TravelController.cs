using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(Client.Constants.ControllerUris.TravelControllerUri)]
public sealed class TravelController(ILogger<TravelController> logger)
    : ApiControllerBase(logger)
{
    [HttpPost(Client.Constants.TravelController.ObtainDirections)]
    public async Task<IImmutableList<Libs.FuelPrices.Core.ViewModels.TravelObtainedModel>> ObtainDirectionsAsync(
        [AsParameters] Libs.FuelPrices.Core.ViewModels.TravelQueryModel travelQueryModel,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] Libs.FuelPrices.Core.Settings.SettingsRoot settings)
    {
        Libs.FuelPrices.Core.JsonObjects.GoogleMaps.Directions.DistanceApiRootJson? DistanceApiResult = await

#if false //DEBUG
            System.Text.Json.JsonSerializer.DeserializeAsync<DistanceApiRootJson>(
                System.IO.File.OpenRead(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Distance.json")));
#else
            LoadJsonAsync<Libs.FuelPrices.Core.JsonObjects.GoogleMaps.Directions.DistanceApiRootJson>(
                httpClientFactory
                , settings.GoogleMapsPlatform.Directions.GetUri(travelQueryModel.Origin, travelQueryModel.Destination, settings.GoogleMapsPlatform.ApiKey)
                , HttpContext.RequestAborted);
#endif

        if (DistanceApiResult == null || DistanceApiResult.Status == null)
        {
            Logger.LogError("La petición de ruta a Google Maps ha devuelto null para '{Origin}' y '{Destination}'.", travelQueryModel.Origin, travelQueryModel.Destination);

            return ImmutableArray<Libs.FuelPrices.Core.ViewModels.TravelObtainedModel>.Empty;
        }

        if (!(DistanceApiResult.Routes?.Length > 0))
        {
            Logger.LogError("La petición de ruta a Google Maps no ha devuelto ningún resultado para '{Origin}' y '{Destination}'.", travelQueryModel.Origin, travelQueryModel.Destination);

            return ImmutableArray<Libs.FuelPrices.Core.ViewModels.TravelObtainedModel>.Empty;
        }

        var ToReturn = (
            from Route in DistanceApiResult.Routes
            from Leg in Route.Legs
            select new Libs.FuelPrices.Core.ViewModels.TravelObtainedModel()
            {
                FromPlace = Leg.StartAddress,
                ToPlace = Leg.EndAddress,
                Summary = Route.Summary,
                Duration = Leg.Duration,
                Distance = Leg.Distance,
                Steps = Leg.Steps.Select(Step => new Libs.FuelPrices.Core.ViewModels.StepObtainedModel()
                {
                    //Instructions = HtmlHelper.ParseHtml(Step.HtmlInstructions),
                    Locations = Step.Polyline.Locations(),
                }).ToArray(),
                GoogleMapsUri = settings.GoogleMapsPlatform.Maps.GetUri(Leg.StartAddress, Leg.EndAddress),
            }).ToImmutableArray();

        return ToReturn;
    }

    [HttpPost(Client.Constants.TravelController.ObtainGasStations)]
    public async Task<IImmutableList<Libs.FuelPrices.Core.ViewModels.GasStationInfoModel>> ObtainGasStationsAsync(
        [FromBody] Libs.FuelPrices.Core.ViewModels.GasStationQueryModel filter,
        [FromServices] Libs.FuelPrices.Services.ObtainFuelPricesService obtainFuelPricesService)
        => await obtainFuelPricesService.ObtainDataAsync(filter);

    private async Task<TJson?> LoadJsonAsync<TJson>(
        IHttpClientFactory httpClientFactory
        , string requestUri
        , CancellationToken cancellationToken = default)
    {
        HttpClient WebClient = httpClientFactory.CreateClient("Default");

        TJson? Resultado = await WebClient.GetFromJsonAsync<TJson>(requestUri, JsonOptions, cancellationToken);

        if (Resultado == null)
            Logger.LogError("La petición de {RequestUri} ha devuelto null.", requestUri);

        return await Task.FromResult(Resultado);
    }
}
