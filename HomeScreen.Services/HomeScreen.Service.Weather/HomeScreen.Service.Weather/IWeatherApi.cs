namespace HomeScreen.Service.Weather;

public interface IWeatherApi
{
    Task<CurrentForecastReply> GetForecast(float latitude, float longitude, CancellationToken cancellationToken);

    Task<HourlyForecastReply> GetHourlyForecast(float latitude, float longitude, CancellationToken cancellationToken);

    Task<DailyForecastReply> GetDailyForecast(float latitude, float longitude, CancellationToken cancellationToken);
}
