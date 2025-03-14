using HomeScreen.AppHost;
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

var openObserveEmail = builder.AddParameter("OpenObserveEmail");
var openObservePassword = builder.AddParameter("OpenObservePassword", true);

var openObserve = builder.AddOpenObserve("OpenObserve", openObserveEmail, openObservePassword).WithDataVolume();

var redis = builder
    .AddRedis("homescreen-redis")
    .WithImageTag("latest")
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WithDataVolume()
    .WithPersistence();

var sqlServer = builder
    .AddSqlServer("homescreen-sqlserver", mssqlPassword)
    .WithImageTag("2022-latest")
    .WithImageRegistry("mcr.microsoft.com")
    .WithImage("mssql/server")
    .WithContainerRuntimeArgs("--cap-add=SYS_PTRACE", "--platform=linux/amd64")
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WithDataVolume();

var mediaDb = sqlServer.AddDatabase("homescreen-media");
var dashboardDb = sqlServer.AddDatabase("homescreen-dashboard");

var mediaMigration = builder
    .AddProject<HomeScreen_Database_MediaDb_Migrations>("homescreen-media-migrations")
    .WithReference(mediaDb)
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WaitFor(mediaDb);

var location = builder
    .AddProject<HomeScreen_Service_Location>("homescreen-service-location")
    .AsHttp2Service()
    .WithReference(redis)
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WaitFor(redis)
    .WithEnvironment("MappingService", mappingService)
    .WithEnvironment("AZURE_MAPS_SUBSCRIPTION_KEY", mapsKey)
    .WithEnvironment("AZURE_CLIENT_ID", clientId);


var media = builder
    .AddProject<HomeScreen_Service_Media>("homescreen-service-media")
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(mediaDb)
    .WithReference(location)
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
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
    .AsHttp2Service()
    .WithReference(redis)
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WaitFor(redis);

var common = builder
    .AddProject<HomeScreen_Web_Common_Server>("homescreen-web-common-server")
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(weather)
    .WithReference(media)
    .WithReference(openObserve)
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WaitFor(openObserve)
    .WaitFor(media)
    .WaitFor(weather)
    .WaitFor(redis);

var dashboard = builder
    .AddProject<HomeScreen_Web_Dashboard_Server>("homescreen-web-dashboard-server")
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(dashboardDb)
    .WithReference(common)
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WaitFor(redis)
    .WaitFor(dashboardDb)
    .WaitFor(common)
    .WithEnvironment("CommonAddress", commonAddress)
    .WithEnvironment("SlideshowAddress", slideshowAddress);

var slideshow = builder
    .AddProject<HomeScreen_Web_Slideshow_Server>("homescreen-web-slideshow-server")
    .AsHttp2Service()
    .WithReference(redis)
    .WithReference(common)
    .WithOpenObserve(openObserve)
    .WaitFor(openObserve)
    .WaitFor(redis)
    .WaitFor(common)
    .WithEnvironment("CommonAddress", commonAddress)
    .WithEnvironment("DashboardAddress", dashboardAddress);

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
