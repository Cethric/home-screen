using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Dashboard.Server.Entities;

public record Config
{
    [Required]
    public required string CommonUrl { get; init; }

    [Required]
    public required string SlideshowUrl { get; init; }
}
