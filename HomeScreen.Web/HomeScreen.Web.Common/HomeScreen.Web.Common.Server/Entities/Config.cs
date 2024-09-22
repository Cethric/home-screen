using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Common.Server.Entities;

public record Config
{
    [Required]
    public required string SentryDsn { get; init; }
}
