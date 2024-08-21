using Grpc.Core;
using HomeScreen.Service.Proto.Services;
using HomeScreen.Service.Weather;
using HomeScreen.Web.Common.Server.Entities;
using DailyForecast = HomeScreen.Web.Common.Server.Entities.DailyForecast;
using HourlyForecast = HomeScreen.Web.Common.Server.Entities.HourlyForecast;

namespace HomeScreen.Web.Common.Server.Services;

public class WeatherApi(ILogger<WeatherApi> logger, WeatherGrpcClient client) : IWeatherApi
{
    public async Task<WeatherForecast?> GetCurrentForecast(
        float longitude,
        float latitude,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Request current weather forecast");
        var result = await client.CurrentForecastAsync(
            new ForecastRequest { Longitude = longitude, Latitude = latitude },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(2).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        if (result is null) return null;
        return new WeatherForecast
        {
            FeelsLikeTemperature = result.FeelsLikeTemperature,
            MaxTemperature = result.MaxTemperature,
            MinTemperature = result.MinTemperature,
            ChanceOfRain = result.ChanceOfRain,
            AmountOfRain = result.AmountOfRain,
            WeatherCode = result.WeatherCode,
        };
    }

    public async Task<IEnumerable<HourlyForecast>?> GetHourlyForecast(
        float longitude,
        float latitude,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Request hourly weather forecast");
        var result = await client.HourlyForecastAsync(
            new ForecastRequest { Longitude = longitude, Latitude = latitude },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(2).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        return result?.Forecast.Select(
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

    public async Task<IEnumerable<DailyForecast>?> GetDailyForecast(
        float longitude,
        float latitude,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Request daily weather forecast");
        var result = await client.DailyForecastAsync(
            new ForecastRequest { Longitude = longitude, Latitude = latitude },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(2).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        return result?.Forecast.Select(
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
