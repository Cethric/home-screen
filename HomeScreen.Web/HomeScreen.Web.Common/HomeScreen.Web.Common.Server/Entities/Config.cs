using System.ComponentModel.DataAnnotations;

namespace HomeScreen.Web.Common.Server.Entities;

public record OtlpConfig
{
    [Required] public required string Endpoint { get; init; }

    [Required] public required string Headers { get; init; }

    [Required] public required string Attributes { get; init; }
}

public record RumConfig
{
    [Required] public required string Endpoint { get; init; }
    [Required] public required string ClientToken { get; init; }
    [Required] public required string OrganizationIdentifier { get; init; }
    [Required] public required bool InsecureHttp { get; init; }
}

public record Config
{
    [Required] public required OtlpConfig OtlpConfig { get; init; }
[Required] public required RumConfig RumConfig { get; init; }
}