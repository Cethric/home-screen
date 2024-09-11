using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Common.Server.Entities;

public record Config
{
    [Required]
    public string MediaUrl { get; init; } = string.Empty;

    [Required]
    public string SentryDsn { get; init; } = string.Empty;
}
