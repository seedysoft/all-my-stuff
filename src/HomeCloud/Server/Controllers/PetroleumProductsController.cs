using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seedysoft.HomeCloud.Shared;
using Seedysoft.HomeCloud.Shared.ViewModels;
using System.Collections.Immutable;

namespace Seedysoft.HomeCloud.Server.Controllers;

[Route(ControllerUris.PetroleumProductsControllerUri)]
public class PetroleumProductsController : ApiControllerBase
{
    public PetroleumProductsController(ILogger<PetroleumProductsController> logger) : base(logger) => Logger = logger;

    [HttpGet()]
    [Route("[action]")]
    public async Task<IImmutableList<IdDescRecord>> PetroleumProductsForFilterAsync(
        [FromServices] Carburantes.Infrastructure.Data.CarburantesDbContext carburantesDbContext)
    {
        IQueryable<IdDescRecord> Query =
            from p in carburantesDbContext.ProductosPetroliferos
            orderby p.NombreProducto
            select new IdDescRecord(p.IdProducto, p.NombreProducto);

        return ImmutableArray.Create(await Query.ToArrayAsync());
    }
}
