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
        builder.AddSeqEndpoint("homescreen-seq");
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

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder, string version)
    {
        builder.Logging.AddOpenTelemetry(
            logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            }
        );

        builder.Services.AddOpenTelemetry()
            .WithMetrics(
                metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation();
                }
            )
            .WithTracing(
                tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                        // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                        .AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation();
                }
            );

        builder.AddOpenTelemetryExporters(version);

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(
        this IHostApplicationBuilder builder,
        string version
    )
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry()
                .UseOtlpExporter()
                .ConfigureResource(
                    c =>
                    {
                        c.AddAttributes(
                            new List<KeyValuePair<string, object>>
                            {
                                new("service.default.version", GitVersionInformation.InformationalVersion),
                                new("service.version", version)
                            }
                        );
                    }
                );
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder, string version)
    {
        builder.Services.AddHealthChecks()
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

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health", new HealthCheckOptions { });

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
        }

        return app;
    }
}
