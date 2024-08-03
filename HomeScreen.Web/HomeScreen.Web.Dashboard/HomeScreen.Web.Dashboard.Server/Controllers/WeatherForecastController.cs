using Microsoft.AspNetCore.Mvc;

namespace HomeScreen.Web.Dashboard.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
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
    };

[HttpGet(Name = "GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{
    logger.LogInformation("Attempting to get WeatherForecast");
    return Enumerable.Range(1, 5)
        .Select(
            index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }
        )
        .ToArray();
}
}
