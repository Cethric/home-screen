using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
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
        builder.AddSeqEndpoint(
            "homescreen-seq",
            settings =>
            {
                settings.ServerUrl = builder.Configuration.GetConnectionString("homescreen-seq");
                settings.DisableHealthChecks = false;
            }
        );
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

        var endpoint = builder.Configuration.GetConnectionString("homescreen-seq");
        if (endpoint is null)
        {
            return builder;
        }

        if (string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
        {
            return builder;
        }

        var otlpEndpoint = new UriBuilder(endpoint) { Path = "/ingest/otlp/" };

        builder.Services.AddOpenTelemetry()
            // .UseOtlpExporter()
            // .UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, otlpEndpoint.Uri)
            .ConfigureResource(
                c =>
                {
                    c.AddTelemetrySdk()
                        .AddService(
                            typeof(Extensions).Assembly.GetName().Name ?? "HomeScreen.ServiceDefaults",
                            "HomeScreen",
                            GitVersionInformation.InformationalVersion
                        )
                        .AddService(
                            Assembly.GetEntryAssembly()!.GetName().Name ?? "HomeScreen.Service",
                            "HomeScreen",
                            version
                        );
                }
            )
            .WithMetrics(
                metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter(
                            options =>
                            {
                                options.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!);
                                options.Protocol = OtlpExportProtocol.Grpc;
                            }
                        );
                }
            )
            .WithTracing(
                tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                        .AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddOtlpExporter(
                            options =>
                            {
                                otlpEndpoint.Path = "/ingest/otlp/v1/traces";
                                options.Endpoint = otlpEndpoint.Uri;
                                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                            }
                        );
                }
            )
            .WithLogging(
                logging =>
                {
                    logging.AddOtlpExporter(
                            options =>
                            {
                                otlpEndpoint.Path = "/ingest/otlp/v1/logs";
                                options.Endpoint = otlpEndpoint.Uri;
                                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                            }
                        )
                        .AddConsoleExporter();
                }
            );

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
            app.MapHealthChecks("/health", new HealthCheckOptions());

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

            app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml");
            app.UseSwaggerUi(p => p.DocumentPath = "/swagger/{documentName}/swagger.yaml");
            app.UseApimundo(p => p.DocumentPath = "/swagger/{documentName}/swagger.json");
            app.UseDeveloperExceptionPage();
        }

        return app;
    }
}
