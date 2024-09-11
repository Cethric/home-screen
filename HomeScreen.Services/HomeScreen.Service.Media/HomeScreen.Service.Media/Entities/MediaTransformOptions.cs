namespace HomeScreen.Service.Media.Entities;

public class MediaTransformOptions
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public bool Blur { get; set; }
    public MediaTransformOptionsFormat Format { get; set; }
}
