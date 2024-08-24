using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HomeScreen.Database.MediaDb.Entities;

[PrimaryKey(nameof(MediaId))]
public class MediaEntry
{
    public MediaEntry()
    {
    }

    public MediaEntry(FileInfo file, string hash)
    {
        MediaId = Guid.NewGuid();
        OriginalHash = hash;
        OriginalFile = file.FullName;
        Notes = string.Empty;
        CapturedUtc = DateTimeOffset.UtcNow;
        CapturedOffset = TimeSpan.Zero;
        Latitude = 0;
        LatitudeDirection = LatitudeDirection.Invalid;
        Longitude = 0;
        LongitudeDirection = LongitudeDirection.Invalid;
        Enabled = false;
    }

    [Required]
    public Guid MediaId { get; set; }

    [Required]
    [MaxLength(260)]
    public string OriginalFile { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string OriginalHash { get; set; } = string.Empty;

    [Required]
    public double Latitude { get; set; }

    [Required]
    public LatitudeDirection LatitudeDirection { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public LongitudeDirection LongitudeDirection { get; set; }

    [Required]
    [MaxLength(4096)]
    public string LocationLabel { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset CapturedUtc { get; set; }

    [Required]
    public TimeSpan CapturedOffset { get; set; }

    [Required]
    [MaxLength(1048576)]
    public string Notes { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;
}
