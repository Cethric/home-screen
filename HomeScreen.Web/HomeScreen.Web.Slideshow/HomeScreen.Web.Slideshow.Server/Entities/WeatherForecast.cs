namespace HomeScreen.Web.Slideshow.Server.Entities;

public class WeatherForecast
{
    public float FeelsLikeTemperature { get; set; }
    public float MaxTemperature { get; set; }
    public float MinTemperature { get; set; }
    public float ChanceOfRain { get; set; }
    public float AmountOfRain { get; set; }
    public string WeatherCode { get; set; } = string.Empty;
}
