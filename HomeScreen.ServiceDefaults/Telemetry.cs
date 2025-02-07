using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HomeScreen.ServiceDefaults;

public static class Telemetry
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder, string version)
    {
        if (string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"])) return builder;

        builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            }
        );

        builder
            .Services.AddOpenTelemetry()
            .UseOtlpExporter()
            .ConfigureResource(c =>
                {
                    c
                        .AddTelemetrySdk()
                        .AddService(
                            typeof(Telemetry).Assembly.GetName().Name ?? "HomeScreen.ServiceDefaults",
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
            .WithLogging()
            .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation();
                }
            )
            .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddGrpcClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation();
                    if (builder.Environment.IsDevelopment())
                        tracing.SetSampler<AlwaysOnSampler>();
                    else
                        tracing.SetSampler<AlwaysOffSampler>();
                }
            );

        return builder;
    }
}
