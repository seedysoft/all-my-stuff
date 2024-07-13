﻿using Microsoft.AspNetCore.Mvc;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
#pragma warning disable CS9113 // Parameter is unread.
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
#pragma warning restore CS9113 // Parameter is unread.
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        })
        .ToArray();
    }
}
