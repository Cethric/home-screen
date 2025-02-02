namespace HomeScreen.AppHost;

public class OpenObserveResource(string name, ParameterResource user, ParameterResource password)
    : ContainerResource(name), IResourceWithConnectionString
{
    public const string ConnectionGrpcEndpoint = "grpc";
public const string ConnectionHttpEndpoint = "http";

public ParameterResource UserParameter { get; } = user;
public ParameterResource PasswordParameter { get; } = password;

private EndpointReference? _endpoint;

public EndpointReference EndpointReference => _endpoint ??= new EndpointReference(this, "http");

public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create(
    $"Endpoint={EndpointReference.Property(EndpointProperty.IPV4Host)}:{EndpointReference.Property(EndpointProperty.Port)}" +
    $";Username={UserParameter}" +
    $";Password={PasswordParameter}"
);
}

public static class OpenObserve
{
    public static IResourceBuilder<OpenObserveResource> AddOpenObserve(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        IResourceBuilder<ParameterResource> email,
        IResourceBuilder<ParameterResource>? password = null,
        int? port = null
    )
    {
        var passwordParameter = password?.Resource
                                ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(
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

        return builder.AddResource(resource)
            .WithImage("zinclabs/openobserve")
            .WithImageRegistry("public.ecr.aws")
            .WithImageTag("latest-simd")
            .WithHttpEndpoint(port: port, targetPort: 5080, name: OpenObserveResource.ConnectionHttpEndpoint)
            .WithEndpoint(targetPort: 5081, scheme: "http", name: OpenObserveResource.ConnectionGrpcEndpoint)
            .WithEnvironment(context =>
            {
                context.EnvironmentVariables["ZO_ROOT_USER_EMAIL"] = resource.UserParameter;
                context.EnvironmentVariables["ZO_ROOT_USER_PASSWORD"] = resource.PasswordParameter;
            })
            .WithOtlpExporter();
    }

    public static IResourceBuilder<OpenObserveResource> WithDataVolume(
        this IResourceBuilder<OpenObserveResource> builder
    )
    {
        return builder
            .WithEnvironment("ZO_META_STORE", "sqlite")
            .WithEnvironment("ZO_DATA_DIR", "/data")
            .WithEnvironment("ZO_DATA_DB_DIR", "/data/db")
            .WithEnvironment("ZO_DATA_WAL_DIR", "/data/wal")
            .WithEnvironment("ZO_DATA_STREAM_DIR", "/data/stream")
            .WithEnvironment("ZO_DATA_IDX_DIR", "/data/wal-idx")
            .WithEnvironment("ZO_LOCAL_MODE", "true")
            .WithEnvironment("ZO_LOCAL_MODE_STORAGE", "disk")
            .WithVolume("/data");
    }
}