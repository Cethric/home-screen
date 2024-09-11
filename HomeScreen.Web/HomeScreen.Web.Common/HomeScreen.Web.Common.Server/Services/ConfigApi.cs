using System.Diagnostics;
using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public class ConfigApi(ILogger<ConfigApi> logger, IConfiguration configuration) : IConfigApi
{
    private static ActivitySource ActivitySource => new(nameof(ConfigApi));

    public Task<Config?> ResolveConfig(CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity(nameof(ResolveConfig), ActivityKind.Client);


        return Task.FromResult<Config?>(
            new Config { MediaUrl = "", SentryDsn = configuration["CLIENT_SENTRY_DSN"] ?? string.Empty, }
        );
    }
}
