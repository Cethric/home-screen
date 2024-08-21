using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeScreen.Web.Dashboard.Server.Endpoints;

public record Config
{
    public string CommonUrl { get; init; } = string.Empty;
}

public static class ConfigEndpoint
{
    private static string _commonUrl = string.Empty;

    public static void RegisterConfigEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/config").WithTags("config").WithName("Config").WithGroupName("Config");

        group.MapGet("/", Config).WithName(nameof(Config));


        _commonUrl = (app.Environment.IsProduction()
                         ? app.Configuration.GetValue<string>("CommonAddress")
                         : app.Configuration.GetSection("services")
                             .GetSection("homescreen-web-common-server")
                             .GetSection("http")
                             .GetChildren()
                             .FirstOrDefault()
                             ?.Value) ??
                     string.Empty;
    }

    private static Task<Ok<Config>> Config() => Task.FromResult(TypedResults.Ok(new Config { CommonUrl = _commonUrl }));
}
