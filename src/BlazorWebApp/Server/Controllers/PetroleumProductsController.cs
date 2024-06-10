using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seedysoft.BlazorWebApp.Client;
using System.Collections.Immutable;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(ControllerUris.PetroleumProductsControllerUri)]
public sealed class PetroleumProductsController : ApiControllerBase
{
    public PetroleumProductsController(ILogger<PetroleumProductsController> logger) : base(logger) => Logger = logger;

    [HttpGet()]
    [Route("[action]")]
    public async Task<IImmutableList<Client.ViewModels.IdDescRecord>> PetroleumProductsForFilterAsync(
        [FromServices] Carburantes.Infrastructure.Data.CarburantesDbContext carburantesDbContext)
    {
        IQueryable<Client.ViewModels.IdDescRecord> Query =
            from p in carburantesDbContext.ProductosPetroliferos
            orderby p.NombreProducto
            select new Client.ViewModels.IdDescRecord(p.IdProducto, p.NombreProducto);

        return ImmutableArray.Create(await Query.ToArrayAsync());
    }
}
