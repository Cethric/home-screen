using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeScreen.Web.Common.Server.Endpoints;

public record Config
{
    public string MediaUrl { get; init; } = string.Empty;
}

public static class ConfigEndpoint
{
    public static void RegisterConfigEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/config").WithTags("config").WithName("Config").WithGroupName("Config");

        group.MapGet("/", Config).WithName(nameof(Config));
    }

    private static Task<Ok<Config>> Config() =>
        Task.FromResult(TypedResults.Ok(new Config { MediaUrl = "https://media" }));
}
