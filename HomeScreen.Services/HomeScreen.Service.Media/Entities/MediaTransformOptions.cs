namespace HomeScreen.Service.Media.Entities;

public class MediaTransformOptions
{
    public long Width { get; set; }
    public long Height { get; set; }
    public float BlurRadius { get; set; }
    public string ImageFormat { get; set; } = string.Empty;
}
