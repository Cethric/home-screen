using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeScreen.Web.Common.Server.Endpoints;

public record Config
{
    public string MediaUrl { get; init; } = string.Empty;
}

public static class ConfigEndpoint
{
    private static ActivitySource ActivitySource => new(nameof(ConfigEndpoint));

    public static void RegisterConfigEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/config").WithTags("config").WithName("Config").WithGroupName("Config");

        group.MapGet("/", Config).WithName(nameof(Config));
    }

    private static Task<Ok<Config>> Config()
    {
        using var activity = ActivitySource.StartActivity("Config", ActivityKind.Client);
        return Task.FromResult(TypedResults.Ok(new Config { MediaUrl = "https://media" }));
    }
}
