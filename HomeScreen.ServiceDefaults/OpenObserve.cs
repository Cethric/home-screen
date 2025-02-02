using System.Data.Common;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HomeScreen.ServiceDefaults;

internal sealed class OpenObserveHealthCheck(OpenObserveSettings settings, HttpClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken
    )
    {
        var response = await client.GetAsync(settings.Endpoint, cancellationToken);
        return response.IsSuccessStatusCode ? HealthCheckResult.Healthy() : HealthCheckResult.Degraded();
    }
}

public class OpenObserveSettings
{
    internal const string DefaultConfigSectionName = "OpenObserve:Telemetry";

    public string? Endpoint { get; set; }
    public string? Credentials { get; set; }
    public bool DisableHealthChecks { get; set; }

    internal void ParseConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string is empty.");
        }

        var builder = new DbConnectionStringBuilder()
        {
            ConnectionString = connectionString
        };

        if (builder.TryGetValue("Endpoint", out var endpoint) is false)
        {
            throw new InvalidOperationException("Connection string has invalid endpoint.");
        }

        Endpoint = endpoint.ToString();

        if (builder.TryGetValue("Username", out var username) && builder.TryGetValue("Password", out var password))
        {
            Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{username}:{password}"
                )
            );
        }
    }
}

public class OpenObserveSettingsAdapter(OpenObserveSettings settings)
{
    public string Endpoint => settings.Endpoint ?? string.Empty;
    public string Credentials => settings.Credentials ?? string.Empty;
}

public static class OpenObserve
{
    public static void AddOpenObserve(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<OpenObserveSettings>? configureSettings = null) =>
        AddOpenObserve(
            builder,
            OpenObserveSettings.DefaultConfigSectionName,
            configureSettings,
            connectionName,
            serviceKey: null);

    public static void AddKeyedOpenObserve(
        this IHostApplicationBuilder builder,
        string name,
        Action<OpenObserveSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        AddOpenObserve(
            builder,
            $"{OpenObserveSettings.DefaultConfigSectionName}:{name}",
            configureSettings,
            connectionName: name,
            serviceKey: name);
    }

    private static void AddOpenObserve(
        this IHostApplicationBuilder builder,
        string configurationSectionName,
        Action<OpenObserveSettings>? configureSettings,
        string connectionName,
        object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new OpenObserveSettings();

        builder.Configuration
            .GetSection(configurationSectionName)
            .Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is { } connectionString)
        {
            Console.WriteLine($"Conn {connectionName} {connectionString}");
            settings.ParseConnectionString(connectionString);
        }

        configureSettings?.Invoke(settings);

        builder.Services.AddTransient(_ => new OpenObserveSettingsAdapter(settings));

        if (settings.DisableHealthChecks is false)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<OpenObserveHealthCheck>(
                    name: serviceKey is null ? "OpenObserve" : $"OpenObserve_{connectionName}",
                    failureStatus: default,
                    tags: ["open-observe"]
                );
        }
    }
}