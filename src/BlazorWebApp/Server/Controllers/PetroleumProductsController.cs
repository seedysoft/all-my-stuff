using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(Client.Constants.ControllerUris.PetroleumProductsControllerUri)]
public sealed class PetroleumProductsController(ILogger<PetroleumProductsController> logger)
    : ApiControllerBase(logger)
{
    [HttpGet()]
    [Route("[action]")]
    public async Task<IImmutableList<Libs.FuelPrices.Core.ViewModels.IdDescRecord>> PetroleumProductsForFilterAsync(
        [FromServices] Libs.FuelPrices.Services.ObtainFuelPricesService obtainFuelPricesService)
        => await obtainFuelPricesService.GetPetroleumProductsAsync();
}
