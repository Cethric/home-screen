using HomeScreen.Database.MediaDb;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Configuration;
using HomeScreen.Service.Media.Services;
using HomeScreen.ServiceDefaults;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddMediaDb();


builder.Services.AddSingleton(
    new MediaDirectories
    {
        MediaSourceDir =
            builder.Configuration.GetValue<string>("MediaSourceDir") ??
            Path.Combine(Path.GetTempPath(), "DashHome", "Source"),
        MediaCacheDir = builder.Configuration.GetValue<string>("MediaCacheDir") ??
                        Path.Combine(Path.GetTempPath(), "DashHome", "Cache")
    }
);
builder.Services.AddSingleton(
    new AzureConfig
    {
        MapSecretKey = builder.Configuration.GetValue<string>("AZURE_MAPS_SUBSCRIPTION_KEY") ?? string.Empty
    }
);

// Add services to the container.
builder.Services.AddInfrastructure();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.MapGrpcService<MediaService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.Run();
