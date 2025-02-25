using System.Text;
using Microsoft.Extensions.Hosting;

namespace HomeScreen.AppHost;

public class OpenObserveResource(string name, ParameterResource user, ParameterResource password)
    : ContainerResource(name), IResourceWithConnectionString
{
    public const string ConnectionGrpcEndpoint = "grpc";
    public const string ConnectionHttpEndpoint = "http";

    private EndpointReference? _endpoint;

    public ParameterResource UserParameter { get; } = user;
    public ParameterResource PasswordParameter { get; } = password;

    public EndpointReference EndpointReference => _endpoint ??= new EndpointReference(this, ConnectionHttpEndpoint);

    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create(
        $"Endpoint={EndpointReference.Property(EndpointProperty.IPV4Host)}:{EndpointReference.Property(EndpointProperty.Port)}" +
        $";Username={UserParameter}" +
        $";Password={PasswordParameter}"
    );
}

public static class OpenObserve
{
    private const string OtelServiceNameAnnotation = "otel-service-name";
    private const string OtelServiceInstanceIdAnnotation = "otel-service-instance-id";
    
    private const string DashboardOtlpGrpcUrlVariableName = "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL";
    private const string DashboardOtlpHttpUrlVariableName = "DOTNET_DASHBOARD_OTLP_HTTP_ENDPOINT_URL";
    private const string DashboardOtlpUrlDefaultValue = "http://host.docker.internal:18889";

    public static IResourceBuilder<OpenObserveResource> AddOpenObserve(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        IResourceBuilder<ParameterResource> email,
        IResourceBuilder<ParameterResource>? password = null,
        int? port = null
    )
    {
        var passwordParameter = password?.Resource ??
                                ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(
                                    builder,
                                    $"{name}-password",
                                    true,
                                    true,
                                    true,
                                    true,
                                    1,
                                    1,
                                    1,
                                    1
                                );
        var resource = new OpenObserveResource(name, email.Resource, passwordParameter);

        return builder
            .AddResource(resource)
            .WithImage("zinclabs/openobserve")
            .WithImageRegistry("public.ecr.aws")
            .WithImageTag("latest-simd")
            .WithHttpEndpoint(
                port,
                targetPort: 5080,
                env: "ZO_HTTP_PORT",
                name: OpenObserveResource.ConnectionHttpEndpoint
            )
            .WithEndpoint(
                targetPort: 5081,
                env: "ZO_GRPC_PORT",
                scheme: "http",
                name: OpenObserveResource.ConnectionGrpcEndpoint
            )
            .WithEnvironment(context =>
                {
                    context.EnvironmentVariables["ZO_ROOT_USER_EMAIL"] = resource.UserParameter;
                    context.EnvironmentVariables["ZO_ROOT_USER_PASSWORD"] = resource.PasswordParameter;
                }
            )
            .WithOtlpExporter();
    }

    public static IResourceBuilder<OpenObserveResource> WithDataVolume(
        this IResourceBuilder<OpenObserveResource> builder
    ) =>
        builder
            .WithEnvironment("ZO_META_STORE", "sqlite")
            .WithEnvironment("ZO_DATA_DIR", "/data")
            .WithEnvironment("ZO_DATA_DB_DIR", "/data/db")
            .WithEnvironment("ZO_DATA_WAL_DIR", "/data/wal")
            .WithEnvironment("ZO_DATA_STREAM_DIR", "/data/stream")
            .WithEnvironment("ZO_DATA_IDX_DIR", "/data/wal-idx")
            .WithEnvironment("ZO_LOCAL_MODE", "true")
            .WithEnvironment("ZO_LOCAL_MODE_STORAGE", "disk")
            .WithVolume("/data");

    private static void AddOtlpEnvironment(
        IResource resource,
        IHostEnvironment environment,
        IResourceBuilder<OpenObserveResource> otelCollector
    )
    {
        // Configure OpenTelemetry in projects using environment variables.
        // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/configuration/sdk-environment-variables.md

        resource.Annotations.Add(
            new EnvironmentCallbackAnnotation(context =>
                {
                    if (context.ExecutionContext.IsPublishMode)
                        // REVIEW:  Do we want to set references to an imaginary otlp provider as a requirement?
                        return;

                    var dashboardOtlpGrpcUrl =
                        otelCollector.Resource.GetEndpoint(OpenObserveResource.ConnectionGrpcEndpoint);
                    var dashboardOtlpHttpUrl =
                        otelCollector.Resource.GetEndpoint(OtelCollectorResource.OtelCollectorHttpEndpoint);

                    // The dashboard can support OTLP/gRPC and OTLP/HTTP endpoints at the same time, but it can
                    // only tell resources about one of the endpoints via environment variables.
                    // If both OTLP/gRPC and OTLP/HTTP are available then prefer gRPC.
                    if (string.IsNullOrWhiteSpace(dashboardOtlpGrpcUrl.Url) is false)
                        SetOtelEndpointAndProtocol(context.EnvironmentVariables, dashboardOtlpGrpcUrl.Url, "grpc");
                    else if (string.IsNullOrWhiteSpace(dashboardOtlpHttpUrl.Url) is false)
                        SetOtelEndpointAndProtocol(
                            context.EnvironmentVariables,
                            dashboardOtlpHttpUrl.Url,
                            "http/protobuf"
                        );
                    else
                        // No endpoints provided to host. Use default value for URL.
                        SetOtelEndpointAndProtocol(context.EnvironmentVariables, DashboardOtlpUrlDefaultValue, "grpc");

                    context.EnvironmentVariables["OTEL_RESOURCE_ATTRIBUTES"] =
                        $"service.instance.id={{{{- index .Annotations \"{OtelServiceInstanceIdAnnotation}\" -}}}}";
                    context.EnvironmentVariables["OTEL_SERVICE_NAME"] =
                        $"{{{{- index .Annotations \"{OtelServiceNameAnnotation}\" -}}}}";

                    var passwordString =
                        $"{otelCollector.Resource.UserParameter}:{otelCollector.Resource.PasswordParameter}";
                    context.EnvironmentVariables["OTEL_EXPORTER_OTLP_HEADERS"] =
                        $"Authorization=Bearer ${Convert.ToBase64String(Encoding.UTF8.GetBytes(passwordString))}";

                    var bearer = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(
                            $"{otelCollector.Resource.UserParameter.Value}:{otelCollector.Resource.PasswordParameter.Value}"
                        )
                    );
                    context.EnvironmentVariables["OTEL_EXPORTER_OTLP_HEADERS"] = $"Authorization=Bearer {bearer}";

                    // Configure OTLP to quickly provide all data with a small delay in development.
                    if (environment.IsDevelopment())
                    {
                        // Set a small batch schedule delay in development.
                        // This reduces the delay that OTLP exporter waits to sends telemetry and makes the dashboard telemetry pages responsive.
                        const string value = "1000"; // milliseconds
                        context.EnvironmentVariables["OTEL_BLRP_SCHEDULE_DELAY"] = value;
                        context.EnvironmentVariables["OTEL_BSP_SCHEDULE_DELAY"] = value;
                        context.EnvironmentVariables["OTEL_METRIC_EXPORT_INTERVAL"] = value;

                        // Configure trace sampler to send all traces to the dashboard.
                        context.EnvironmentVariables["OTEL_TRACES_SAMPLER"] = "always_on";
                        // Configure metrics to include exemplars.
                        context.EnvironmentVariables["OTEL_METRICS_EXEMPLAR_FILTER"] = "trace_based";
                    }
                }
            )
        );
        return;

        static void SetOtelEndpointAndProtocol(
            Dictionary<string, object> environmentVariables,
            string url,
            string protocol
        )
        {
            environmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = new HostUrl(url);
            environmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = protocol;
        }
    }

    public static IResourceBuilder<T> WithOpenObserve<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<OpenObserveResource> openObserve,
        string? connectionName = null
    ) where T : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(openObserve.Resource);
        builder.WithReference(openObserve, connectionName);
        AddOtlpEnvironment(builder.Resource, builder.ApplicationBuilder.Environment, openObserve);
        return builder;
    }
}
