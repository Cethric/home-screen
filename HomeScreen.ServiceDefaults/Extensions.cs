using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HomeScreen.ServiceDefaults;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder, string version)
    {
        builder.ConfigureOpenTelemetry(version);
#if !SwaggerBuild
        builder.Logging.AddConsole();
        builder.AddRedisOutputCache("homescreen-redis");
        builder.AddRedisDistributedCache("homescreen-redis");
#else
        builder.Services.AddDistributedMemoryCache();
#endif

        builder.AddDefaultHealthChecks(version);

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(
            http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler(
                    config =>
                    {
                        config.AttemptTimeout.Timeout = TimeSpan.FromMinutes(5);
                        config.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(10);
                        config.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(30);
                    }
                );

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            }
        );

        return builder;
    }
}
