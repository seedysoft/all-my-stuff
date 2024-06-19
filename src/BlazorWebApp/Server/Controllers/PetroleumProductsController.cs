using Microsoft.AspNetCore.Mvc;
using Seedysoft.BlazorWebApp.Client;
using Seedysoft.Carburantes.CoreLib.ViewModels;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(ControllerUris.PetroleumProductsControllerUri)]
public sealed class PetroleumProductsController : ApiControllerBase
{
    public PetroleumProductsController(ILogger<PetroleumProductsController> logger) : base(logger) => Logger = logger;

    [HttpGet()]
    [Route("[action]")]
    public async Task<IdDescRecord[]> PetroleumProductsForFilterAsync(
        [FromServices] CarburantesLib.Services.ObtainDataCronBackgroundService obtainDataCronBackgroundService)
        => await obtainDataCronBackgroundService.GetPetroleumProductsForFilterAsync();
}
