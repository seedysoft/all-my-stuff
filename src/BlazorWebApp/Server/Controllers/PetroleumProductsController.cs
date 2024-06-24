using Microsoft.AspNetCore.Mvc;
using Seedysoft.BlazorWebApp.Client;
using System.Collections.Immutable;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(ControllerUris.PetroleumProductsControllerUri)]
public sealed class PetroleumProductsController : ApiControllerBase
{
    public PetroleumProductsController(ILogger<PetroleumProductsController> logger) : base(logger) => Logger = logger;

    [HttpGet()]
    [Route("[action]")]
    public async Task<IImmutableList<Libs.FuelPrices.Core.ViewModels.IdDescRecord>> PetroleumProductsForFilterAsync(
        [FromServices] Libs.FuelPrices.Services.ObtainFuelPricesService obtainFuelPricesService)
        =>
        await obtainFuelPricesService.GetPetroleumProductsAsync();
}
