using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Common.Server.Entities;

public record OtlpConfig
{
    [Required]
    public required string Endpoint { get; init; }

    [Required]
    public required string Headers { get; init; }

    [Required]
    public required string Attributes { get; init; }
}

public record Config
{
    [Required]
    public required OtlpConfig OtlpConfig { get; init; }
}
