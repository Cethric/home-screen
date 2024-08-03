using HomeScreen.Service.Weather.Generated.Entities;

namespace HomeScreen.Service.Weather;

public class WeatherApi(ILogger<WeatherApi> logger, IOpenMeteoClient openMeteoClient) : IWeatherApi
{
    public async Task<CurrentForecastReply> GetForecast(
        float latitude,
        float longitude,
        CancellationToken cancellationToken
    )
{
    logger.LogInformation(
        "Attempting to request new weather information for {Latitude}, {Longitude}",
        latitude,
        longitude
    );
    var result = await openMeteoClient.ForecastAsync(
        latitude,
        longitude,
        current: [CurrentParameter.Apparent_temperature],
        daily:

        [
            DailyParameter.Apparent_temperature_max,
            DailyParameter.Apparent_temperature_min,
            DailyParameter.Weather_code,
            DailyParameter.Precipitation_probability_mean
        ],
        timezone: "Australia/Sydney",
        forecast_days: 1,
        past_days: 0,
        cancellationToken: cancellationToken
    );
    logger.LogInformation("Weather information returned from request");
    return new CurrentForecastReply
    {
        FeelsLikeTemperature = result.Result.Current?.Apparent_temperature ?? 0,
        MaxTemperature = result.Result.Daily?.Apparent_temperature_max?.FirstOrDefault() ?? 0,
        MinTemperature = result.Result.Daily?.Apparent_temperature_min?.FirstOrDefault() ?? 0,
        ChanceOfRain = result.Result.Daily?.Precipitation_probability_max?.FirstOrDefault() ?? 0,
        AmountOfRain = result.Result.Daily?.Precipitation_probability_max?.FirstOrDefault() ?? 0,
        WeatherCode = WmoToString(
            result.Result.Daily?.Weather_code?.FirstOrDefault() ??
            HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._0
        )
    };
}

public async Task<HourlyForecastReply> GetHourlyForecast(
    float latitude,
    float longitude,
    CancellationToken cancellationToken
)
{
    logger.LogInformation(
        "Attempting to request the hourly forecast information for {Latitude}, {Longitude}",
        latitude,
        longitude
    );
    var response = await openMeteoClient.ForecastAsync(
        latitude,
        longitude,
        hourly:

        [
            HourlyParameter.Apparent_temperature,
            HourlyParameter.Precipitation,
            HourlyParameter.Precipitation_probability,
            HourlyParameter.Wind_direction_10m,
            HourlyParameter.Wind_speed_10m,
            HourlyParameter.Wind_gusts_10m,
            HourlyParameter.Is_day,
            HourlyParameter.Cloud_cover
        ],
        timezone: "Australia/Sydney",
        forecast_days: 1,
        forecast_hours: 24,
        past_days: 0,
        cancellationToken: cancellationToken
    );
    if (response.Result.Hourly != null)
    {
        var forecast = new HourlyForecastReply();
        for (var i = response.Result.Hourly.Time.Count - 1; i >= 0; i--)
        {
            forecast.Forecast.Add(
                new HourlyForecast
                {
                    Time = response.Result.Hourly.Time[i].ToUnixTimeMilliseconds(),
                    ApparentTemperature = response.Result.Hourly.Apparent_temperature![i],
                    Precipitation = response.Result.Hourly.Precipitation![i],
                    PrecipitationProbability = response.Result.Hourly.Precipitation_probability![i],
                    WindDirection = response.Result.Hourly.Wind_direction_10m![i],
                    WindSpeed = response.Result.Hourly.Wind_speed_10m![i],
                    WindGusts = response.Result.Hourly.Wind_gusts_10m![i],
                    IsDay = response.Result.Hourly.Is_day![i] == 1,
                    CloudCover = response.Result.Hourly.Cloud_cover![i],
                }
            );
        }

        return forecast;
    }

    throw new AggregateException(
        new ArgumentOutOfRangeException(nameof(latitude), latitude, "Unable to find hourly forecast at latitude"),
        new ArgumentOutOfRangeException(nameof(longitude), longitude, "Unable to find hourly forecast at longitude")
    );
}

public async Task<DailyForecastReply> GetDailyForecast(
    float latitude,
    float longitude,
    CancellationToken cancellationToken
)
{
    logger.LogInformation(
        "Attempting to request the daily forecast information for {Latitude}, {Longitude}",
        latitude,
        longitude
    );
    var response = await openMeteoClient.ForecastAsync(
        latitude,
        longitude,
        daily:

        [
            DailyParameter.Apparent_temperature_min,
            DailyParameter.Apparent_temperature_max,
            DailyParameter.Daylight_duration,
            DailyParameter.Sunrise,
            DailyParameter.Sunset,
            DailyParameter.Uv_index_clear_sky_max,
            DailyParameter.Uv_index_max,
            DailyParameter.Weather_code,
            DailyParameter.Precipitation_sum,
            DailyParameter.Precipitation_probability_max,
            DailyParameter.Precipitation_probability_min
        ],
        timezone: "Australia/Sydney",
        cancellationToken: cancellationToken
    );

    if (response.Result.Daily != null)
    {
        var forecast = new DailyForecastReply();
        for (var i = response.Result.Daily.Time.Count - 1; i >= 0; i--)
        {
            forecast.Forecast.Add(
                new DailyForecast
                {
                    Time =
                        new DateTimeOffset(
                            response.Result.Daily.Time[i].ToDateTime(TimeOnly.MinValue),
                            TimeSpan.Zero
                        ).ToUnixTimeMilliseconds(),
                    ApparentTemperatureMin = response.Result.Daily.Apparent_temperature_min![i],
                    ApparentTemperatureMax = response.Result.Daily.Apparent_temperature_max![i],
                    DaylightDuration = response.Result.Daily.Daylight_duration![i],
                    Sunrise = response.Result.Daily.Sunrise![i].ToUnixTimeMilliseconds(),
                    Sunset = response.Result.Daily.Sunset![i].ToUnixTimeMilliseconds(),
                    UvIndexClearSkyMax = response.Result.Daily.Uv_index_clear_sky_max![i],
                    UvIndexMax = response.Result.Daily.Uv_index_max![i],
                    WeatherCode = (WmoWeatherCode)(int)response.Result.Daily.Weather_code![i],
                    WeatherCodeLabel = WmoToString(response.Result.Daily.Weather_code[i]),
                    PrecipitationSum = response.Result.Daily.Precipitation_sum![i],
                    PrecipitationProbabilityMax = response.Result.Daily.Precipitation_probability_max![i],
                    PrecipitationProbabilityMin = response.Result.Daily.Precipitation_probability_min![i],
                }
            );
        }

        return forecast;
    }

    throw new AggregateException(
        new ArgumentOutOfRangeException(nameof(latitude), latitude, "Unable to find hourly forecast at latitude"),
        new ArgumentOutOfRangeException(nameof(longitude), longitude, "Unable to find hourly forecast at longitude")
    );
}

private string WmoToString(HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode wmo)
{
    return wmo switch
    {
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._0 => "Clear",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._1 => "Mostly Clear",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._2 => "Partly Clear",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._3 => "Overcast",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._45 => "Fog",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._48 => "Rime Fog",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._51 => "Light Drizzle",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._53 => "Medium Drizzle",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._55 => "Heavy Drizzle",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._56 => "Light Freezing Drizzle",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._57 => "Heavy Freezing Drizzle",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._61 => "Light Rain",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._63 => "Medium Rain",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._65 => "Heavy Rain",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._66 => "Light Freezing Rain",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._67 => "Heavy Freezing Rain",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._71 => "Light Snow",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._73 => "Medium Snow",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._75 => "Heavy Snow",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._77 => "Grainy Snow",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._80 => "Light Rain Shower",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._81 => "Medium Rain Shower",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._82 => "Heavy Rain Shower",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._85 => "Light Snow Shower",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._86 => "Heavy Snow Shower",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._95 => "Thunderstorm",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._96 => "Thunderstorm with some rain",
        HomeScreen.Service.Weather.Generated.Entities.WmoWeatherCode._99 => "Thunderstorm with heavy rain",
        _ => "Unknown"
    };
}
}
