using HomeScreen.Database.MediaDb;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Configuration;
using HomeScreen.Service.Media.Entities;
using HomeScreen.Service.Media.Infrastructure.Media;
using HomeScreen.Service.Media.Services;
using HomeScreen.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


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

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.MapGrpcService<MediaService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

//    public int Width { get; set; }
// public int Height { get; set; }
// public float BlurRadius { get; set; }
// public MediaTransformOptionsFormat Format { get; set; }

app.MapGet(
    "/download",
    async (
        [FromQuery] Guid mediaId,
        [FromQuery] int width,
        [FromQuery] int height,
        [FromQuery] bool blur,
        [FromQuery] MediaTransformOptionsFormat format,
        IMediaApi mediaApi,
        CancellationToken cancellationToken = default
    ) =>
    {
        var result = await mediaApi.GetTransformedMedia(
            mediaId,
            new MediaTransformOptions { Width = width, Height = height, Blur = blur, Format = format },
            cancellationToken
        );

        return result is null
            ? Results.NotFound()
            : Results.File(result.Open(FileMode.Open, FileAccess.Read), format.TransformFormatToMime());
    }
);

app.Run();
