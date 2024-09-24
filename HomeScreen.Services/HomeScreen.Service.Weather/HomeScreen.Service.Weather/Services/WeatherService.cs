using System.Diagnostics;
using Grpc.Core;
using HomeScreen.Service.Weather.Generated.Entities;

namespace HomeScreen.Service.Weather.Services;

public class WeatherService(ILogger<WeatherService> logger, IWeatherApi weatherApi) : Weather.WeatherBase
{
    private static ActivitySource ActivitySource => new(nameof(WeatherService));

    public override async Task<CurrentForecastReply> CurrentForecast(ForecastRequest request, ServerCallContext context)
    {
        using var activity = ActivitySource.StartActivity("CurrentForecast", ActivityKind.Client);
        logger.LogDebug("Get current Forecast: {Request}", request);
        try
        {
            var response = await weatherApi.GetForecast(request.Latitude, request.Longitude, context.CancellationToken);
            return response;
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Failed to get current forecast");
            throw;
        }
    }

    public override async Task<HourlyForecastReply> HourlyForecast(ForecastRequest request, ServerCallContext context)
    {
        using var activity = ActivitySource.StartActivity("HourlyForecast", ActivityKind.Client);
        logger.LogDebug("Get hourly Forecast: {Request}", request);
        var response = await weatherApi.GetHourlyForecast(
            request.Latitude,
            request.Longitude,
            context.CancellationToken
        );
        return response;
    }

    public override async Task<DailyForecastReply> DailyForecast(ForecastRequest request, ServerCallContext context)
    {
        using var activity = ActivitySource.StartActivity("DailyForecast", ActivityKind.Client);
        logger.LogDebug("Get daily Forecast: {Request}", request);
        var response = await weatherApi.GetDailyForecast(
            request.Latitude,
            request.Longitude,
            context.CancellationToken
        );
        return response;
    }
}
