using Microsoft.AspNetCore.Mvc;

namespace Seedysoft.HomeCloud.Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecastController(ILogger<WeatherForecastController> logger) : base(logger) { }

    [HttpGet]
    public IEnumerable<Shared.WeatherForecast> Get()
    {
        Random rng = new();
        return Enumerable.Range(1, 5).Select(index => new Shared.WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
