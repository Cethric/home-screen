using Grpc.Core;

namespace HomeScreen.Service.Weather.Services;

public class WeatherService(ILogger<WeatherService> logger, IWeatherApi weatherApi) : Weather.WeatherBase
{
    public override async Task<CurrentForecastReply> CurrentForecast(ForecastRequest request, ServerCallContext context)
    {
        logger.LogInformation($"Get current Forecast: {request}");
        var response = await weatherApi.GetForecast(request.Latitude, request.Longitude, context.CancellationToken);
        return response;
    }

    public override async Task<HourlyForecastReply> HourlyForecast(ForecastRequest request, ServerCallContext context)
    {
        logger.LogInformation($"Get hourly Forecast: {request}");
        var response = await weatherApi.GetHourlyForecast(
            request.Latitude,
            request.Longitude,
            context.CancellationToken
        );
        return response;
    }

    public override async Task<DailyForecastReply> DailyForecast(ForecastRequest request, ServerCallContext context)
    {
        logger.LogInformation($"Get daily Forecast: {request}");
        var response = await weatherApi.GetDailyForecast(
            request.Latitude,
            request.Longitude,
            context.CancellationToken
        );
        return response;
    }
}
