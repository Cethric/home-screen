using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("HomeScreenSqlPassword", secret: true);

var mediaSource = builder.AddParameter("MediaSourceDir");
var mediaCache = builder.AddParameter("MediaCacheDir");

var mapsKey = builder.AddParameter("AzureMapsSecret", true);
var clientId = builder.AddParameter("AzureClientID", true);


var seq = builder.AddSeq("homescreen-seq", 9090).WithImageTag("latest").WithOtlpExporter().WithDataVolume();
var redis = builder.AddRedis("homescreen-redis")
    .WithRedisCommander()
    .WithImageTag("latest")
    .WithOtlpExporter()
    .WithDataVolume()
    .WithPersistence();

var sqlServer = builder.AddSqlServer("homescreen-sqlserver", postgresPassword)
    .WithImageTag("latest")
    .WithOtlpExporter()
    .WithDataVolume();

var mediaDb = sqlServer.AddDatabase("homescreen-media");
var dashboardDb = sqlServer.AddDatabase("homescreen-dashboard");

builder.AddProject<HomeScreen_Database_MediaDb_Migrations>("homescreen-media-migrations")
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(mediaDb);


var media = builder.AddProject<HomeScreen_Service_Media>("homescreen-service-media")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(mediaDb)
    .WithEnvironment("MediaSourceDir", mediaSource)
    .WithEnvironment("MediaCacheDir", mediaCache)
    .WithEnvironment("MappingService", "Nominatim")
    .WithEnvironment("AZURE_MAPS_SUBSCRIPTION_KEY", mapsKey)
    .WithEnvironment("AZURE_CLIENT_ID", clientId)
    .WithAnnotation(new ContainerMountAnnotation("cache", "/cache", ContainerMountType.Volume, false))
    .WithAnnotation(new ContainerMountAnnotation("media", "/media", ContainerMountType.Volume, true));

var weather = builder.AddProject<HomeScreen_Service_Weather>("homescreen-service-weather")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis);

var common = builder.AddProject<HomeScreen_Web_Common_Server>("homescreen-web-common-server")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(weather)
    .WithReference(media);

builder.AddProject<HomeScreen_Web_Slideshow_Server>("homescreen-web-slideshow-server")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(common);

builder.AddProject<HomeScreen_Web_Dashboard_Server>("homescreen-web-dashboard-server")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(common)
    .WithReference(dashboardDb);

var app = builder.Build();
app.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();
