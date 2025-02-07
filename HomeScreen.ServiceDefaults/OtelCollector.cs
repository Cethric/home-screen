using System.Data.Common;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HomeScreen.ServiceDefaults;

internal sealed class OtelCollectorHealthCheck(OtelCollectorSettings settings, HttpClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken
    )
{
    var response = await client.GetAsync(settings.EndpointHttp, cancellationToken);
    return response.IsSuccessStatusCode ? HealthCheckResult.Healthy() : HealthCheckResult.Degraded();
}
}

public class OtelCollectorSettings
{
    internal const string DefaultConfigSectionName = "OtelCollector:Telemetry";

    public string? EndpointHttp { get; set; }
    public string? EndpointGrpc { get; set; }
    public string? Credentials { get; set; }
    public bool DisableHealthChecks { get; set; }

    internal void ParseConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string is empty.");

        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

        if (builder.TryGetValue("EndpointGrpc", out var endpointGrpc) is false)
            throw new InvalidOperationException("Connection string has invalid grpc endpoint.");

        EndpointGrpc = endpointGrpc.ToString();

        if (builder.TryGetValue("EndpointHttp", out var endpointHttp) is false)
            throw new InvalidOperationException("Connection string has invalid http endpoint.");

        EndpointHttp = endpointHttp.ToString();

        if (builder.TryGetValue("Username", out var username) && builder.TryGetValue("Password", out var password))
            Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    }
}

public class OtelCollectorSettingsAdapter(OtelCollectorSettings settings)
{
    public string EndpointGrpc => settings.EndpointGrpc ?? string.Empty;
public string EndpointHttp => settings.EndpointHttp ?? string.Empty;
public string Credentials => settings.Credentials ?? string.Empty;
}

public static class OtelCollector
{
    public static void AddOtelCollector(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<OtelCollectorSettings>? configureSettings = null
    ) =>
        AddOtelCollector(
            builder,
            OtelCollectorSettings.DefaultConfigSectionName,
            configureSettings,
            connectionName,
            null
        );

    public static void AddKeyedOtelCollector(
        this IHostApplicationBuilder builder,
        string name,
        Action<OtelCollectorSettings>? configureSettings = null
    )
    {
        ArgumentNullException.ThrowIfNull(name);

        AddOtelCollector(
            builder,
            $"{OtelCollectorSettings.DefaultConfigSectionName}:{name}",
            configureSettings,
            name,
            name
        );
    }

    private static void AddOtelCollector(
        this IHostApplicationBuilder builder,
        string configurationSectionName,
        Action<OtelCollectorSettings>? configureSettings,
        string connectionName,
        object? serviceKey
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new OtelCollectorSettings();

        builder.Configuration.GetSection(configurationSectionName).Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is { } connectionString)
            settings.ParseConnectionString(connectionString);

        configureSettings?.Invoke(settings);

        builder.Services.AddTransient(_ => new OtelCollectorSettingsAdapter(settings));

        if (settings.DisableHealthChecks is false)
            builder
                .Services.AddHealthChecks()
                .AddCheck<OtelCollectorHealthCheck>(
                    serviceKey is null ? "OtelCollector" : $"OtelCollector_{connectionName}",
                    default,
                    ["otel-collector"]
                );
    }
}
