using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[ApiController]
public abstract class ApiControllerBase(ILogger logger) : ControllerBase
{
    protected virtual ILogger Logger { get; init; } = logger;

    protected internal JsonSerializerOptions JsonOptions { get; } = new() { };
}
