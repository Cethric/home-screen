using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public interface IWeatherApi
{
    Task<WeatherForecast?> GetCurrentForecast(float longitude, float latitude, CancellationToken cancellationToken);

    Task<IEnumerable<HourlyForecast>?> GetHourlyForecast(
        float longitude,
        float latitude,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<DailyForecast>?> GetDailyForecast(
        float longitude,
        float latitude,
        CancellationToken cancellationToken
    );
}
