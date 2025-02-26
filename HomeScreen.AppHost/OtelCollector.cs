using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HomeScreen.AppHost;

public class OtelCollectorResource(string name, ParameterResource user, ParameterResource password)
    : ContainerResource(name), IResourceWithConnectionString
{
    public const string OtelCollectorGrpcEndpoint = "OtelCollectorGRPC";
public const string OtelCollectorHttpEndpoint = "OtelCollectorHttp";
private EndpointReference? _endpointGrpc;

private EndpointReference? _endpointHttp;

public ParameterResource UserParameter { get; } = user;
public ParameterResource PasswordParameter { get; } = password;

public EndpointReference EndpointReferenceHttp =>
    _endpointHttp ??= new EndpointReference(this, OtelCollectorHttpEndpoint);

public EndpointReference EndpointReferenceGrpc =>
    _endpointGrpc ??= new EndpointReference(this, OtelCollectorGrpcEndpoint);

public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create(
    $"EndpointHttp={EndpointReferenceHttp.Property(EndpointProperty.Host)}:{EndpointReferenceHttp.Property(EndpointProperty.Port)}" +
    $"EndpointGrpc={EndpointReferenceGrpc.Property(EndpointProperty.Host)}:{EndpointReferenceGrpc.Property(EndpointProperty.Port)}" +
    $";Username={UserParameter}" +
    $";Password={PasswordParameter}"
);
}

public static class OtelCollector
{
    private const string OtelServiceNameAnnotation = "otel-service-name";
    private const string OtelServiceInstanceIdAnnotation = "otel-service-instance-id";


    private const string DashboardOtlpGrpcUrlVariableName = "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL";
    private const string DashboardOtlpHttpUrlVariableName = "DOTNET_DASHBOARD_OTLP_HTTP_ENDPOINT_URL";
    private const string DashboardOtlpUrlDefaultValue = "http://host.docker.internal:18889";

    public static IResourceBuilder<OtelCollectorResource> AddOtelCollector(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        IResourceBuilder<ParameterResource>? username = null,
        IResourceBuilder<ParameterResource>? password = null
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
        var usernameParameter = username?.Resource ??
                                ParameterResourceBuilderExtensions.CreateGeneratedParameter(
                                    builder,
                                    $"{name}-username",
                                    false,
                                    new GenerateParameterDefault
                                    {
                                        Numeric = false,
                                        Special = false,
                                        Lower = true,
                                        Upper = true,
                                        MinLower = 1,
                                        MinUpper = 1,
                                        MinLength = 5
                                    }
                                );

        var resource = new OtelCollectorResource(name, usernameParameter, passwordParameter);

        return builder
            .AddResource(resource)
            .WithImage("open-telemetry/opentelemetry-collector-releases/opentelemetry-collector-contrib")
            .WithImageRegistry("ghcr.io")
            .WithImageTag("0.118.0")
            .WithEndpoint(targetPort: 4317, scheme: "http", name: OtelCollectorResource.OtelCollectorGrpcEndpoint)
            .WithEndpoint(targetPort: 4318, scheme: "http", name: OtelCollectorResource.OtelCollectorHttpEndpoint)
            .WithArgs(
                "--config=yaml:receivers::otlp::protocols: {" +
                "grpc: { endpoint: 0.0.0.0:4317 }," +
                "http: { " +
                "include_metadata: true, " +
                "endpoint: 0.0.0.0:4318, " +
                "cors: { allowed_origins: [ '*' ] }, " +
                "compression_algorithms: [ '', 'gzip', 'zstd', 'zlib', 'snappy', 'deflate', 'lz4' ] " +
                "}" +
                "}",
                "--config=yaml:exporters::debug: {}",
                "--config=yaml:service::extensions: [ ]"
            )
            .WithArgs(ctx =>
                {
                    var key = builder.Configuration["AppHost:OtlpApiKey"];
                    var aspireGrpc = builder.Configuration[DashboardOtlpGrpcUrlVariableName];
                    var aspireHttp = builder.Configuration[DashboardOtlpHttpUrlVariableName];

                    var exporter = "otlp/aspire";
                    if (!string.IsNullOrWhiteSpace(aspireGrpc))
                    {
                        ctx.Args.Add(
                            $"--config=yaml:exporters::otlp/aspire: {{ " +
                            $"endpoint: {aspireGrpc.Replace("localhost", "host.docker.internal")}, " +
                            $"tls::insecure_skip_verify: true, " +
                            $"headers: {{ " +
                            $"x-otlp-api-key: {key}, " +
                            $"}} " +
                            $"}}"
                        );
                    }
                    else if (!string.IsNullOrWhiteSpace(aspireHttp))
                    {
                        exporter = "otlphttp/aspire";
                        ctx.Args.Add(
                            $"--config=yaml:exporters::otlphttp/aspire: {{ " +
                            $"endpoint: {aspireHttp.Replace("localhost", "host.docker.internal")}, " +
                            $"tls::insecure_skip_verify: true, " +
                            $"headers: {{ " +
                            $"x-otlp-api-key: {key}, " +
                            $"}} " +
                            $"}}"
                        );
                    }
                    else
                    {
                        ctx.Args.Add(
                            $"--config=yaml:exporters::otlp/aspire: {{ " +
                            $"endpoint: {DashboardOtlpUrlDefaultValue}, " +
                            $"tls::insecure: true, " +
                            $"headers: {{ " +
                            $"x-otlp-api-key: {key}, " +
                            $"}}  " +
                            $"}}"
                        );
                    }

                    ctx.Args.Add(
                        $"--config=yaml:service::pipelines::traces: {{ receivers: [ otlp ], exporters: [ {exporter} ] }}"
                    );
                    ctx.Args.Add(
                        $"--config=yaml:service::pipelines::metrics: {{ receivers: [ otlp ], exporters: [ {exporter} ] }}"
                    );
                    ctx.Args.Add(
                        $"--config=yaml:service::pipelines::logs: {{ receivers: [ otlp ], exporters: [ {exporter} ] }}"
                    );
                }
            );
    }

    public static IResourceBuilder<OtelCollectorResource> WithOpenObserve(
        this IResourceBuilder<OtelCollectorResource> builder,
        IResourceBuilder<OpenObserveResource> openObserve
    ) =>
        builder.WithArgs(async ctx =>
            {
                var aspireGrpc = builder.ApplicationBuilder.Configuration[DashboardOtlpGrpcUrlVariableName];
                var aspireHttp = builder.ApplicationBuilder.Configuration[DashboardOtlpHttpUrlVariableName];

                var exporter = string.IsNullOrWhiteSpace(aspireGrpc) && !string.IsNullOrWhiteSpace(aspireHttp)
                    ? "otlphttp/aspire"
                    : "otlp/aspire";

                var endpoint = openObserve.GetEndpoint("http");
                var host = endpoint.ContainerHost;
                var port = await endpoint.Property(EndpointProperty.Port).GetValueAsync(ctx.CancellationToken);

                var bearer = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        $"{openObserve.Resource.UserParameter.Value}:{openObserve.Resource.PasswordParameter.Value}"
                    )
                );
                ctx.Args.Add(
                    $"--config=yaml:exporters::otlphttp/openobserve: {{ " +
                    $"endpoint: http://{host}:{port}/api/default, " +
                    $"tls::insecure: true, " +
                    $"headers: {{ " +
                    $"Authorization: Basic {bearer}, " +
                    $"stream-name: default " +
                    $"}} " +
                    $"}}"
                );

                ctx.Args.Add(
                    $"--config=yaml:service::pipelines::traces: {{ receivers: [ otlp ], exporters: [ {exporter}, otlphttp/openobserve ] }}"
                );
                ctx.Args.Add(
                    $"--config=yaml:service::pipelines::metrics: {{ receivers: [ otlp ], exporters: [ {exporter}, otlphttp/openobserve ] }}"
                );
                ctx.Args.Add(
                    $"--config=yaml:service::pipelines::logs: {{ receivers: [ otlp ], exporters: [ {exporter}, otlphttp/openobserve ] }}"
                );
            }
        );

    private static void AddOtlpEnvironment(
        IResource resource,
        IConfiguration configuration,
        IHostEnvironment environment,
        IResourceBuilder<OtelCollectorResource> otelCollector
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
                        otelCollector.Resource.GetEndpoint(OtelCollectorResource.OtelCollectorGrpcEndpoint);
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

    public static IResourceBuilder<T> WithOtelCollector<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<OtelCollectorResource> otelCollector,
        string? connectionName = null
    ) where T : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(otelCollector.Resource);
        builder.WithReference(otelCollector, connectionName);
        AddOtlpEnvironment(
            builder.Resource,
            builder.ApplicationBuilder.Configuration,
            builder.ApplicationBuilder.Environment,
            otelCollector
        );
        return builder;
    }
}