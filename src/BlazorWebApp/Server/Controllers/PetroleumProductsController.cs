//using Microsoft.AspNetCore.Mvc;

//namespace Seedysoft.BlazorWebApp.Server.Controllers;

//[Route(Client.Constants.PetroleumProductsUris.Controller)]
//public sealed class PetroleumProductsController(ILogger<PetroleumProductsController> logger) : ApiControllerBase(logger)
//{
//    [HttpGet(Client.Constants.PetroleumProductsUris.Actions.ForFilter)]
//    public async Task<IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero>> ForFilterAsync(
//        [FromServices] Libs.GasStationPrices.Services.ObtainGasStationPricesService obtainGasStationPricesService,
//        CancellationToken cancellationToken)
//        => await obtainGasStationPricesService.GetPetroleumProductsAsync(cancellationToken);
//}
