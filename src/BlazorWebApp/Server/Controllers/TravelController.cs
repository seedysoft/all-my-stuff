using Microsoft.AspNetCore.Mvc;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(Client.Constants.TravelUris.Controller)]
public sealed class TravelController : ApiControllerBase
{
    public TravelController(ILogger<TravelController> logger) : base(logger) => Logger = logger;

    [HttpGet(Client.Constants.TravelUris.Actions.FindPlaces)]
    public async Task<IEnumerable<string>> FindPlacesAsync(
        [AsParameters] string textToFind,
        [FromServices] Libs.GasStationPrices.Services.ObtainGasStationPricesService obtainGasStationPricesService,
        CancellationToken cancellationToken)
        => await obtainGasStationPricesService.FindPlacesAsync(textToFind, cancellationToken);

    [HttpPost(Client.Constants.TravelUris.Actions.GetGasStations)]
    public IAsyncEnumerable<Libs.GasStationPrices.Core.ViewModels.GasStationModel> GetGasStationsAsync(
        [AsParameters] Libs.GasStationPrices.Core.ViewModels.TravelQueryModel travelQueryModel,
        [FromServices] Libs.GasStationPrices.Services.ObtainGasStationPricesService obtainGasStationPricesService,
        CancellationToken cancellationToken)
        => obtainGasStationPricesService.GetGasStationsAsync(travelQueryModel, cancellationToken);
}
