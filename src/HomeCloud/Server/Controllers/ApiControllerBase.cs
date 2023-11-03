using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Seedysoft.HomeCloud.Server.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected virtual ILogger Logger { get; init; }

    protected internal JsonSerializerOptions JsonOptions { get; } = new() { };

    protected ApiControllerBase(ILogger logger) => Logger = logger;
}
