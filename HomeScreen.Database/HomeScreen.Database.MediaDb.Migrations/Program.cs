using HomeScreen.Database.MediaDb;
using HomeScreen.Database.MediaDb.Migrations;
using HomeScreen.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder.AddMediaDb();

var host = builder.Build();

host.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await host.RunAsync();
