using Projects;
using static HomeScreen.AppHost.Extensions.PostgresServer;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServerPassword = builder.AddParameter("homescreen-sqlserver-password", secret: true);
var postgres = builder.AddPostgresServer("homescreen-sqlserver", sqlServerPassword);
var mediaDb = postgres.AddPostgresDatabase("homescreen-mediadb");

var media = builder.AddProject<HomeScreen_Service_Media>("homescreen-service-media")
    .AsHttp2Service()
    .WithReference(mediaDb);

builder.AddProject<HomeScreen_Web_Slideshow_Server>("homescreen-web-slideshow-server")
    .AsHttp2Service()
    .WithReference(media);

builder.AddProject<HomeScreen_Web_Dashboard_Server>("homescreen-web-dashboard-server")
    .AsHttp2Service()
    .WithReference(media);

builder.Build().Run();
