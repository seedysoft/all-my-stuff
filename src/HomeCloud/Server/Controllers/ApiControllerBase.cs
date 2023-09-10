using Microsoft.AspNetCore.Mvc;

namespace Seedysoft.HomeCloud.Server.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected virtual ILogger Logger { get; init; }

    protected ApiControllerBase(ILogger logger) => Logger = logger;
}
