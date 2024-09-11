using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Slideshow.Server.Entities;

public record Config
{
    [Required]
    public string CommonUrl { get; init; } = string.Empty;
}
