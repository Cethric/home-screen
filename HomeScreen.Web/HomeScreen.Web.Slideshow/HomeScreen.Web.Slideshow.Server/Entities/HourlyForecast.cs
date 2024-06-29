namespace HomeScreen.Web.Slideshow.Server.Entities;

public class HourlyForecast
{
    public DateTimeOffset Time { get; set; }
    public float ApparentTemperature { get; set; }
    public float Precipitation { get; set; }
    public float PrecipitationProbability { get; set; }
    public float WindDirection { get; set; }
    public float WindSpeed { get; set; }
    public float WindGusts { get; set; }
    public bool IsDay { get; set; }
    public float CloudCover { get; set; }
}
