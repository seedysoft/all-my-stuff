using Microsoft.AspNetCore.Mvc;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(Client.Constants.TravelUris.Controller)]
public sealed class TravelController : ApiControllerBase
{
    public TravelController(ILogger<TravelController> logger) : base(logger) => Logger = logger;

    //[HttpGet(Client.Constants.TravelUris.Actions.GetMapId)]
    //public async Task<string> GetMapId(
    //    [FromServices] Libs.GoogleApis.Services.PlacesService placesService,
    //    CancellationToken cancellationToken)
    //    => await placesService.GetMapId(cancellationToken);

    [HttpGet(Client.Constants.TravelUris.Actions.FindPlaces)]
    public async Task<IEnumerable<string>> FindPlacesAsync(
        [AsParameters] string textToFind,
        [FromServices] Libs.GoogleApis.Services.PlacesService placesService,
        CancellationToken cancellationToken)
        => await placesService.FindPlacesAsync(textToFind, cancellationToken);

    [HttpPost(Client.Constants.TravelUris.Actions.GetGasStations)]
    public IAsyncEnumerable<Libs.GasStationPrices.ViewModels.GasStationModel> GetGasStationsAsync(
        [AsParameters] Libs.GasStationPrices.ViewModels.TravelQueryModel travelQueryModel,
        [FromServices] Libs.GasStationPrices.Services.ObtainGasStationPricesService obtainGasStationPricesService,
        CancellationToken cancellationToken)
        => obtainGasStationPricesService.GetGasStationsAsync(travelQueryModel, cancellationToken);
}
