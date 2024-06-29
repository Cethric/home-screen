using Grpc.Core;
using HomeScreen.Service.Weather;
using HomeScreen.Web.Slideshow.Server.Entities;
using HomeScreen.Web.Slideshow.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using DailyForecast = HomeScreen.Web.Slideshow.Server.Entities.DailyForecast;
using HourlyForecast = HomeScreen.Web.Slideshow.Server.Entities.HourlyForecast;

namespace HomeScreen.Web.Slideshow.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherGrpcClient client)
    : ControllerBase
{
    [HttpGet(Name = "GetCurrentForecast")]
    [ProducesResponseType<WeatherForecast>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WeatherForecast>> GetCurrentForecast(
        [FromQuery] float longitude,
        [FromQuery] float latitude,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("Request current weather forecast");
        var result = await client.CurrentForecastAsync(
            new ForecastRequest { Longitude = longitude, Latitude = latitude },
            new Metadata(),
            DateTimeOffset.Now.AddMinutes(2).UtcDateTime,
            cancellationToken
        );
        return Ok(
            new WeatherForecast
            {
                FeelsLikeTemperature = result.FeelsLikeTemperature,
                MaxTemperature = result.MaxTemperature,
                MinTemperature = result.MinTemperature,
                ChanceOfRain = result.ChanceOfRain,
                AmountOfRain = result.AmountOfRain,
                WeatherCode = result.WeatherCode,
            }
        );
    }

    [HttpGet(Name = "GetHourlyForecast")]
    [ProducesResponseType<IEnumerable<HourlyForecast>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<HourlyForecast>> GetHourlyForecast(
        [FromQuery] float longitude,
        [FromQuery] float latitude,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("Request hourly weather forecast");
        var result = await client.HourlyForecastAsync(
            new ForecastRequest { Longitude = longitude, Latitude = latitude },
            new Metadata(),
            DateTimeOffset.Now.AddMinutes(2).UtcDateTime,
            cancellationToken
        );
        return result.Forecast.Select(
            forecast => new HourlyForecast
            {
                Time = DateTimeOffset.FromUnixTimeMilliseconds(forecast.Time),
                ApparentTemperature = forecast.ApparentTemperature,
                Precipitation = forecast.Precipitation,
                PrecipitationProbability = forecast.PrecipitationProbability,
                WindDirection = forecast.WindDirection,
                WindSpeed = forecast.WindSpeed,
                WindGusts = forecast.WindGusts,
                IsDay = forecast.IsDay,
                CloudCover = forecast.CloudCover,
            }
        );
    }

    [HttpGet(Name = "GetDailyForecast")]
    [ProducesResponseType<IEnumerable<DailyForecast>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<DailyForecast>> GetDailyForecast(
        [FromQuery] float longitude,
        [FromQuery] float latitude,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("Request daily weather forecast");
        var result = await client.DailyForecastAsync(
            new ForecastRequest { Longitude = longitude, Latitude = latitude },
            new Metadata(),
            DateTimeOffset.Now.AddMinutes(2).UtcDateTime,
            cancellationToken
        );
        return result.Forecast.Select(
            forecast => new DailyForecast
            {
                Time = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(forecast.Time).Date),
                ApparentTemperatureMin = forecast.ApparentTemperatureMin,
                ApparentTemperatureMax = forecast.ApparentTemperatureMax,
                DaylightDuration = forecast.DaylightDuration,
                Sunrise = DateTimeOffset.FromUnixTimeMilliseconds(forecast.Sunrise),
                Sunset = DateTimeOffset.FromUnixTimeMilliseconds(forecast.Sunset),
                UvIndexClearSkyMax = forecast.UvIndexClearSkyMax,
                UvIndexMax = forecast.UvIndexMax,
                WeatherCode = forecast.WeatherCode,
                WeatherCodeLabel = forecast.WeatherCodeLabel,
                PrecipitationSum = forecast.PrecipitationSum,
                PrecipitationProbabilityMax = forecast.PrecipitationProbabilityMax,
                PrecipitationProbabilityMin = forecast.PrecipitationProbabilityMin,
            }
        );
    }
}
