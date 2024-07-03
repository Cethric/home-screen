namespace HomeScreen.Web.Slideshow.Server.Entities;

public class MediaItem
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public MediaItemLocation Location { get; set; } = new();
}
