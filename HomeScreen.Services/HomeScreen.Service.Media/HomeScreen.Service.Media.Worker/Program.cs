using HomeScreen.Database.MediaDb;
using HomeScreen.Service.Location.Client;
using HomeScreen.Service.Media.Common;
using HomeScreen.Service.Media.Worker;
using HomeScreen.Service.Media.Worker.Infrastructure;
using HomeScreen.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder.AddMediaCommon();
builder.AddMediaDb();
builder.AddLocationApi();
builder.AddGrpcHealthCheck("homescreen-service-location");
builder
    .Services.AddScoped<IMediaApi, MediaApi>()
    .AddScoped<IMediaHasher, MediaHasher>()
    .AddScoped<IMediaProcessor, MediaProcessor>()
    .AddScoped<IMediaDateTimeProcessor, MediaDateTimeProcessor>()
    .AddScoped<IMediaLocationProcessor, MediaLocationProcessor>()
    .AddScoped<IMediaColourProcessor, MediaHistogramColourProcessor>()
    .AddScoped<IMediaMetadataReader, MediaMetadataReader>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host
    .Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);

await host.RunAsync();
