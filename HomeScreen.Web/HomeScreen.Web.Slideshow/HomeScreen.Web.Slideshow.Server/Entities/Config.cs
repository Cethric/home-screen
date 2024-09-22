using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Slideshow.Server.Entities;

public record Config
{
    [Required]
    public required string CommonUrl { get; init; }

    [Required]
    public required string DashboardUrl { get; init; }
}
