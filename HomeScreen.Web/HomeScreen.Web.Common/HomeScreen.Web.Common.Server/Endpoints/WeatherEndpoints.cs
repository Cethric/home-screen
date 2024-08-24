using HomeScreen.Web.Common.Server.Entities;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeScreen.Web.Common.Server.Endpoints;

public static class WeatherEndpoints
{
    private static async Task<Results<Ok<WeatherForecast>, NotFound>> CurrentForecast(
        float longitude,
        float latitude,
        IWeatherApi weatherApi,
        CancellationToken cancellationToken
    )
    {
        var result = await weatherApi.GetCurrentForecast(longitude, latitude, cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<IEnumerable<HourlyForecast>>, NotFound>> HourlyForecast(
        float longitude,
        float latitude,
        IWeatherApi weatherApi,
        CancellationToken cancellationToken
    )
    {
        var result = await weatherApi.GetHourlyForecast(longitude, latitude, cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<IEnumerable<DailyForecast>>, NotFound>> DailyForecast(
        float longitude,
        float latitude,
        IWeatherApi weatherApi,
        CancellationToken cancellationToken
    )
    {
        var result = await weatherApi.GetDailyForecast(longitude, latitude, cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    public static void RegisterWeatherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/weather/{longitude:required:float}/{latitude:required:float}")
                       .WithTags("weather")
                       .WithName("Weather")
                       .WithGroupName("Weather");

        group.MapGet("/current", CurrentForecast).WithName(nameof(CurrentForecast));
        group.MapGet("/hourly", HourlyForecast).WithName(nameof(HourlyForecast));
        group.MapGet("/daily", DailyForecast).WithName(nameof(DailyForecast));
    }
}
