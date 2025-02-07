namespace HomeScreen.Web.Common.Server.Entities;

public class MediaItem
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public MediaItemLocation Location { get; set; } = new();
    public double AspectRatio { get; set; }
    public bool Portrait { get; set; }
    public uint BaseB { get; set; }
    public uint BaseG { get; set; }
    public uint BaseR { get; set; }
}
