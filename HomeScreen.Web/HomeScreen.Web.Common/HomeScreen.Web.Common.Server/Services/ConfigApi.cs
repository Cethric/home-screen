using System.Diagnostics;
using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public class ConfigApi(IConfiguration configuration, IWebHostEnvironment environment) : IConfigApi
{
    private static ActivitySource ActivitySource => new(nameof(ConfigApi));

public Task<Config> ResolveConfig(CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity();

    return Task.FromResult<Config>(
        new Config
        {
            OtlpConfig = new OtlpConfig
            {
                Endpoint = environment.IsProduction()
                                 ? "https://otel.homescreen.home-automation.cloud"
                                 : configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? string.Empty,
                Headers = configuration["OTEL_EXPORTER_OTLP_HEADERS"] ?? string.Empty,
                Attributes = configuration["OTEL_RESOURCE_ATTRIBUTES"] ?? string.Empty
            }
        }
    );
}
}
