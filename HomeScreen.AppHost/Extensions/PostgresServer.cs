namespace HomeScreen.AppHost.Extensions;

internal static class PostgresContainerImageTags
{
    public const string Registry = "docker.io";
    public const string Image = "library/postgres";
    public const string Tag = "16.2";
}

public class PostgresServerResource : ContainerResource, IResourceWithConnectionString
{
    internal const string PrimaryEndpointName = "tcp";
    private const string DefaultUserName = "postgres";

    public EndpointReference PrimaryEndpoint { get; }
    public ParameterResource? UserNameParameter { get; }
    public ParameterResource PasswordParameter { get; }

    public PostgresServerResource(string name, ParameterResource? userName, ParameterResource password) : base(name)
    {
        ArgumentNullException.ThrowIfNull(password);

        PrimaryEndpoint = new(this, PrimaryEndpointName);
        UserNameParameter = userName;
        PasswordParameter = password;
    }

    internal ReferenceExpression UserNameReference =>
        UserNameParameter is not null
            ? ReferenceExpression.Create($"{UserNameParameter}")
            : ReferenceExpression.Create($"{DefaultUserName}");

    private ReferenceExpression ConnectionString =>
        ReferenceExpression.Create(
            $"Host={PrimaryEndpoint.Property(EndpointProperty.Host)};Port={PrimaryEndpoint.Property(EndpointProperty.Port)};Username={UserNameReference};Password={PasswordParameter}"
        );

    public ReferenceExpression ConnectionStringExpression
    {
        get
        {
            if (this.TryGetLastAnnotation<ConnectionStringRedirectAnnotation>(out var connectionStringAnnotation))
            {
                return connectionStringAnnotation.Resource.ConnectionStringExpression;
            }

            return ConnectionString;
        }
    }

    public ValueTask<string?> GetConnectionStringAsync(CancellationToken cancellationToken = default)
    {
        if (this.TryGetLastAnnotation<ConnectionStringRedirectAnnotation>(out var connectionStringAnnotation))
        {
            return connectionStringAnnotation.Resource.GetConnectionStringAsync(cancellationToken);
        }

        return ConnectionStringExpression.GetValueAsync(cancellationToken);
    }

    private readonly Dictionary<string, string> _databases = new();

    public IReadOnlyDictionary<string, string> Databases => _databases;

    internal void AddDatabase(string name, string databaseName)
    {
        _databases.TryAdd(name, databaseName);
    }
}

public class PostgresDatabaseResource(string name, string databaseName, PostgresServerResource postgresParentResource)
    : Resource(name), IResourceWithParent<PostgresServerResource>, IResourceWithConnectionString
{
    public PostgresServerResource Parent { get; } = postgresParentResource;

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{Parent};Database={DatabaseName}");

    public string DatabaseName { get; } = databaseName;
}

public static class PostgresServer
{
    private const string UserEnvVarName = "POSTGRES_USER";
    private const string PasswordEnvVarName = "POSTGRES_PASSWORD";

    public static IResourceBuilder<PostgresServerResource> AddPostgresServer(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<ParameterResource>? userName = null,
        IResourceBuilder<ParameterResource>? password = null,
        int? port = null
    )
    {
        var passwordParameter = password?.Resource ??
                                ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(
                                    builder,
                                    $"{name}-password"
                                );

        var postgresServer = new PostgresServerResource(name, userName?.Resource, passwordParameter);
        return builder.AddResource(postgresServer)
            .WithEndpoint(
                hostPort: port,
                containerPort: 5432,
                name: PostgresServerResource.PrimaryEndpointName
            ) // Internal port is always 5432.
            .WithImage(PostgresContainerImageTags.Image, PostgresContainerImageTags.Tag)
            .WithImageRegistry(PostgresContainerImageTags.Registry)
            .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "scram-sha-256")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--auth-host=scram-sha-256 --auth-local=scram-sha-256")
            .WithEnvironment(
                context =>
                {
                    context.EnvironmentVariables[UserEnvVarName] = postgresServer.UserNameReference;
                    context.EnvironmentVariables[PasswordEnvVarName] = postgresServer.PasswordParameter;
                }
            );
    }

    public static IResourceBuilder<PostgresDatabaseResource> AddPostgresDatabase(
        this IResourceBuilder<PostgresServerResource> builder,
        string name,
        string? databaseName = null
    )
    {
        // Use the resource name as the database name if it's not provided
        databaseName ??= name;

        builder.Resource.AddDatabase(name, databaseName);
        var postgresDatabase = new PostgresDatabaseResource(name, databaseName, builder.Resource);
        return builder.ApplicationBuilder.AddResource(postgresDatabase);
    }
}
