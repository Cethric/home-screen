using System.Diagnostics;
using HomeScreen.Web.Dashboard.Server.Entities;
using HomeScreen.Web.Dashboard.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeScreen.Web.Dashboard.Server.Endpoints;

public static class ConfigEndpoint
{
    private static ActivitySource ActivitySource => new(nameof(ConfigEndpoint));

    public static void RegisterConfigEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/config").WithTags("config").WithName("Config").WithGroupName("Config");

        group.MapGet("/", Config).WithName(nameof(Config));
    }

    private static async Task<Results<NotFound, Ok<Config>>> Config(IConfigApi api, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity();
        var result = await api.ResolveConfig(cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }
}
