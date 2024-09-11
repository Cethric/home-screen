using System.Diagnostics;
using HomeScreen.Web.Common.Server.Entities;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeScreen.Web.Common.Server.Endpoints;

public static class ConfigEndpoint
{
    private static ActivitySource ActivitySource => new(nameof(ConfigEndpoint));

    public static void RegisterConfigEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/config").WithTags("config").WithName("Config").WithGroupName("Config");

        group.MapGet("/", Config).WithName(nameof(Config));
    }

    private static async Task<Results<Ok<Config>, NotFound>> Config(IConfigApi api, CancellationToken token)
    {
        using var activity = ActivitySource.StartActivity("Config", ActivityKind.Client);
        var result = await api.ResolveConfig(token);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }
}
