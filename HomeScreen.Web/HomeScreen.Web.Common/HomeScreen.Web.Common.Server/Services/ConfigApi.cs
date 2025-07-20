using System.Diagnostics;
using HomeScreen.ServiceDefaults;
using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public class ConfigApi(
    IConfiguration configuration,
    IWebHostEnvironment environment
) : IConfigApi
{
    private static ActivitySource ActivitySource => new(nameof(ConfigApi));

    public Task<Config> ResolveConfig(CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity();

        return Task.FromResult(
            new Config{}
        );
    }
}
