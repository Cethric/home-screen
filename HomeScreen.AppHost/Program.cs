using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mappingService = builder.AddParameter("MappingService", "Nominatim");

var mssqlPassword = builder.AddParameter("HomeScreenSqlServerPassword", true);

var mediaSource = builder.AddParameter("MediaSourceDir");
var mediaCache = builder.AddParameter("MediaCacheDir");

var mapsKey = builder.AddParameter("AzureMapsSecret", true);
var clientId = builder.AddParameter("AzureClientID", true);

var commonAddress = builder.AddParameter("CommonAddress");
var dashboardAddress = builder.AddParameter("DashboardAddress");
var slideshowAddress = builder.AddParameter("SlideshowAddress");

var redis = builder
    .AddRedis("homescreen-redis")
    .WithOtlpExporter()
    .WithImageTag("latest")
    .WithDataVolume()
    .WithPersistence();

var sqlServer = builder
    .AddSqlServer("homescreen-sqlserver", mssqlPassword)
    .WithOtlpExporter()
    .WithImageTag("2022-latest")
    .WithImageRegistry("mcr.microsoft.com")
    .WithImage("mssql/server")
    .WithContainerRuntimeArgs("--cap-add=SYS_PTRACE", "--platform=linux/amd64")
    .WithDataVolume();

var mediaDb = sqlServer.AddDatabase("homescreen-media");
var dashboardDb = sqlServer.AddDatabase("homescreen-dashboard");

var mediaMigration = builder
    .AddProject<HomeScreen_Database_MediaDb_Migrations>("homescreen-media-migrations")
    .WithOtlpExporter()
    .WithReference(mediaDb)
    .WaitFor(mediaDb);

var location = builder
    .AddProject<HomeScreen_Service_Location>("homescreen-service-location")
    .WithOtlpExporter()
    .AsHttp2Service()
    .WithReference(redis)
    .WaitFor(redis)
    .WithEnvironment("MappingService", mappingService)
    .WithEnvironment("AZURE_MAPS_SUBSCRIPTION_KEY", mapsKey)
    .WithEnvironment("AZURE_CLIENT_ID", clientId);


var media = builder
    .AddProject<HomeScreen_Service_Media>("homescreen-service-media")
    .WithOtlpExporter()
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(mediaDb)
    .WaitFor(redis)
    .WaitFor(mediaDb)
    .WaitForCompletion(mediaMigration)
    .WithEnvironment("MediaSourceDir", mediaSource)
    .WithEnvironment("MediaCacheDir", mediaCache)
    .WithAnnotation(new ContainerMountAnnotation("cache", "/cache", ContainerMountType.Volume, false))
    .WithAnnotation(new ContainerMountAnnotation("media", "/media", ContainerMountType.Volume, true));

var mediaWorker = builder
    .AddProject<HomeScreen_Service_Media_Worker>("homescreen-service-media-worker")
    .WithOtlpExporter()
    .WithReference(redis)
    .WithReference(mediaDb)
    .WithReference(location)
    .WaitFor(redis)
    .WaitFor(mediaDb)
    .WaitFor(location)
    .WaitForCompletion(mediaMigration)
    .WithEnvironment("MediaSourceDir", mediaSource)
    .WithEnvironment("MediaCacheDir", mediaCache)
    .WithAnnotation(new ContainerMountAnnotation("cache", "/cache", ContainerMountType.Volume, false))
    .WithAnnotation(new ContainerMountAnnotation("media", "/media", ContainerMountType.Volume, true));

var weather = builder
    .AddProject<HomeScreen_Service_Weather>("homescreen-service-weather")
    .WithOtlpExporter()
    .AsHttp2Service()
    .WithReference(redis)
    .WaitFor(redis);

var common = builder
    .AddProject<HomeScreen_Web_Common_Server>("homescreen-web-common-server")
    .WithOtlpExporter()
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(weather)
    .WithReference(media)
    .WaitFor(media)
    .WaitFor(mediaWorker)
    .WaitFor(weather)
    .WaitFor(redis)
    .WithHttpHealthCheck("/alive");

var dashboard = builder
    .AddProject<HomeScreen_Web_Dashboard_Server>("homescreen-web-dashboard-server")
    .WithOtlpExporter()
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(dashboardDb)
    .WithReference(common)
    .WaitFor(redis)
    .WaitFor(dashboardDb)
    .WaitFor(common)
    .WithEnvironment("CommonAddress", commonAddress)
    .WithEnvironment("SlideshowAddress", slideshowAddress)
    .WithHttpHealthCheck("/alive");

var slideshow = builder
    .AddProject<HomeScreen_Web_Slideshow_Server>("homescreen-web-slideshow-server")
    .WithOtlpExporter()
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(common)
    .WaitFor(redis)
    .WaitFor(common)
    .WithEnvironment("CommonAddress", commonAddress)
    .WithEnvironment("DashboardAddress", dashboardAddress)
    .WithHttpHealthCheck("/alive");

slideshow.WithReference(dashboard);
dashboard.WithReference(slideshow);

builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            }
        );
    }
);

var app = builder.Build();
app
    .Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);

await app.RunAsync();
