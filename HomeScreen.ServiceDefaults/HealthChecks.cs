using System.Text;
using System.Text.Json;
using Grpc.Health.V1;
using Microsoft.AspNetCore.Http;
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

    public static IHostApplicationBuilder AddGrpcHealthCheck(
        this IHostApplicationBuilder builder,
        string name,
        HealthStatus failureStatus = default
    )
    {
        var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";
        var url = $"{(isHttps ? "https" : "http")}://{name}";
        builder.Services.AddGrpcClient<Health.HealthClient>(o => o.Address = new Uri(url));
        builder.Services.AddHealthChecks().AddCheck<GrpcServiceHealthCheck>(name, failureStatus);

        return builder;
    }

    private sealed class GrpcServiceHealthCheck(Health.HealthClient healthClient) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        )
    {
        var response = await healthClient.CheckAsync(
            new HealthCheckRequest(),
            cancellationToken: cancellationToken
        );

        return response.Status switch
        {
            HealthCheckResponse.Types.ServingStatus.Serving => HealthCheckResult.Healthy(),
            _ => HealthCheckResult.Unhealthy()
        };
    }
}
}