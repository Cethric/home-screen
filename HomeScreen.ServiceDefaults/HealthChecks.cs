using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HomeScreen.ServiceDefaults;

public static class HealthChecks
{
    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder, string version)
    {
        builder
            .Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck(
                "self",
                () => HealthCheckResult.Healthy(
                    data: new Dictionary<string, object>
                    {
                        { "service.defaults.version", GitVersionInformation.InformationalVersion },
                        { "service.version", version }
                    }
                ),
                ["live"]
            );

        return builder;
    }
}
