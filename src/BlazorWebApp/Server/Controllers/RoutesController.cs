using Microsoft.AspNetCore.Mvc;
using Seedysoft.BlazorWebApp.Client;
using Seedysoft.Carburantes.CoreLib.ViewModels;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(ControllerUris.RoutesControllerUri)]
public sealed class RoutesController : ApiControllerBase
{
    public RoutesController(ILogger<RoutesController> logger) : base(logger) => Logger = logger;

    [HttpPost]
    public async Task<RouteObtainedModel[]> ObtainRoutesAsync(
        [AsParameters] RouteQueryModel routeQueryModel,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] Carburantes.CoreLib.Settings.SettingsRoot settings)
    {
        Carburantes.CoreLib.JsonObjects.GoogleMaps.Directions.DistanceApiRootJson? DistanceApiResult = await
#if false //DEBUG
            System.Text.Json.JsonSerializer.DeserializeAsync<DistanceApiRootJson>(
                System.IO.File.OpenRead(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Distance.json")));
#else
            LoadJsonAsync<Carburantes.CoreLib.JsonObjects.GoogleMaps.Directions.DistanceApiRootJson>(
                httpClientFactory,
                settings.GoogleMapsPlatform.Directions.GetUri(routeQueryModel.Origin, routeQueryModel.Destination, settings.GoogleMapsPlatform.ApiKey));
#endif

        if (DistanceApiResult == null || DistanceApiResult.Status == null)
        {
            Logger.LogError("La petición de ruta a Google Maps ha devuelto null para '{Origin}' y '{Destination}'.", routeQueryModel.Origin, routeQueryModel.Destination);

            return [];
        }

        if (!(DistanceApiResult.Routes?.Length > 0))
        {
            Logger.LogError("La petición de ruta a Google Maps no ha devuelto ningún resultado para '{Origin}' y '{Destination}'.", routeQueryModel.Origin, routeQueryModel.Destination);

            return [];
        }

        RouteObtainedModel[] ToReturn = (
            from Route in DistanceApiResult.Routes
            from Leg in Route.Legs
            select new RouteObtainedModel()
            {
                FromPlace = Leg.StartAddress,
                ToPlace = Leg.EndAddress,
                Summary = Route.Summary,
                Duration = Leg.Duration,
                Distance = Leg.Distance,
                Steps = Leg.Steps.Select(Step => new StepObtainedModel()
                {
                    //Instructions = HtmlHelper.ParseHtml(Step.HtmlInstructions),
                    Locations = Step.Polyline.Locations(),
                }).ToArray(),
                GoogleMapsUri = settings.GoogleMapsPlatform.Maps.GetUri(Leg.StartAddress, Leg.EndAddress),
            }).ToArray();

        return ToReturn;
    }

    [HttpPost]
    public async Task<GasStationInfoModel[]> ObtainGasStationsAsync(
        [FromBody] GasStationQueryModel filter
        , [FromServices] CarburantesLib.Services.ObtainDataCronBackgroundService obtainDataCronBackground)
        => await obtainDataCronBackground.GetGasStationInfoAsync(filter);

    private async Task<TJson?> LoadJsonAsync<TJson>(
        IHttpClientFactory httpClientFactory,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        HttpClient WebClient = httpClientFactory.CreateClient("Default");

        TJson? Resultado = await WebClient.GetFromJsonAsync<TJson>(requestUri, JsonOptions, cancellationToken);

        if (Resultado == null)
            Logger.LogError("La petición de {RequestUri} ha devuelto null.", requestUri);

        return await Task.FromResult(Resultado);
    }
}
