using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("HomeScreenSqlPassword", secret: true);

var seq = builder.AddSeq("homescreen-seq", 9090);
var redis = builder.AddRedis("homescreen-redis").WithOtlpExporter();

var postgres = builder.AddSqlServer("homescreen-sqlserver", postgresPassword).WithOtlpExporter();
var mediaDb = postgres.AddDatabase("homescreen-media");
var dashboardDb = postgres.AddDatabase("homescreen-dashboard");

var media = builder.AddProject<HomeScreen_Service_Media>("homescreen-service-media")
    .AsHttp2Service()
    .WithOtlpExporter()
    .WithReference(seq)
    .WithReference(redis)
    .WithReference(mediaDb);

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

builder.Build().Run();
