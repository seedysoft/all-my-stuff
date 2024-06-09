using Microsoft.AspNetCore.Mvc;
using Seedysoft.HomeCloud.Client;

namespace Seedysoft.HomeCloud.Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class WeatherForecastController(ILogger<WeatherForecastController> logger) : ApiControllerBase(logger)
{
    private static readonly string[] Summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    ];

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        Random rng = new();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
