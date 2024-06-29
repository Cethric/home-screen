using HomeScreen.Service.Weather;

namespace HomeScreen.Web.Slideshow.Server.Entities;

public class DailyForecast
{
    public DateOnly Time { get; set; }
    public float ApparentTemperatureMin { get; set; }
    public float ApparentTemperatureMax { get; set; }
    public float DaylightDuration { get; set; }
    public DateTimeOffset Sunrise { get; set; }
    public DateTimeOffset Sunset { get; set; }
    public float UvIndexClearSkyMax { get; set; }
    public float UvIndexMax { get; set; }
    public WmoWeatherCode WeatherCode { get; set; }
    public string WeatherCodeLabel { get; set; } = string.Empty;
    public float PrecipitationSum { get; set; }
    public float PrecipitationProbabilityMax { get; set; }
    public float PrecipitationProbabilityMin { get; set; }
}
