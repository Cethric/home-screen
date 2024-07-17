using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("HomeScreenSqlPassword", secret: true);

var mediaSource = builder.AddParameter("MediaSourceDir");
var mediaCache = builder.AddParameter("MediaCacheDir");

var mapsKey = builder.AddParameter("AzureMapsSecret", true);
var clientId = builder.AddParameter("AzureClientID", true);


var seq = builder.AddSeq("homescreen-seq", 9090).WithOtlpExporter().WithDataVolume();
var redis = builder.AddRedis("homescreen-redis").WithOtlpExporter().WithDataVolume().WithPersistence();

var sqlServer = builder.AddSqlServer("homescreen-sqlserver", postgresPassword).WithOtlpExporter().WithDataVolume();
var mediaDb = sqlServer.AddDatabase("homescreen-media");

builder.AddProject<HomeScreen_Database_MediaDb_Migrations>("homescreen-media-migrations")
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(mediaDb);

var dashboardDb = sqlServer.AddDatabase("homescreen-dashboard");

var media = builder.AddProject<HomeScreen_Service_Media>("homescreen-service-media")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(mediaDb)
    .WithEnvironment("MediaSourceDir", mediaSource)
    .WithEnvironment("MediaCacheDir", mediaCache)
    .WithEnvironment("AZURE_MAPS_SUBSCRIPTION_KEY", mapsKey)
    .WithEnvironment("AZURE_CLIENT_ID", clientId)
    .WithAnnotation(new ContainerMountAnnotation("cache", "/cache", ContainerMountType.Volume, false))
    .WithAnnotation(new ContainerMountAnnotation("media", "/media", ContainerMountType.Volume, true));

var weather = builder.AddProject<HomeScreen_Service_Weather>("homescreen-service-weather")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis);

builder.AddProject<HomeScreen_Web_Slideshow_Server>("homescreen-web-slideshow-server")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(weather)
    .WithReference(media);

builder.AddProject<HomeScreen_Web_Dashboard_Server>("homescreen-web-dashboard-server")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(weather)
    .WithReference(media)
    .WithReference(dashboardDb);

await builder.Build().RunAsync();
