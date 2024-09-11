using HomeScreen.Database.MediaDb;
using HomeScreen.Database.MediaDb.Migrations;
using HomeScreen.ServiceDefaults;
using Sentry.OpenTelemetry;
using Sentry.Profiling;

var builder = Host.CreateApplicationBuilder(args);
using var sentry = SentrySdk.Init(
    options =>
    {
        options.Debug = builder.Environment.IsDevelopment();
        options.TracesSampleRate = 1.0;
        options.ProfilesSampleRate = 1.0;
        options.StackTraceMode = StackTraceMode.Enhanced;
        options.AutoSessionTracking = true;
        options.IsGlobalModeEnabled = true;
        options.ServerName = "MediaDbMigrations";
        options.AddIntegration(new ProfilingIntegration(TimeSpan.FromMilliseconds(500)));
        options.UseOpenTelemetry();
        options.AddEntityFramework();
    }
);

builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder.AddMediaDb();

var host = builder.Build();

host.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await host.RunAsync();
