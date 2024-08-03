using System.Text.Json.Serialization;
using HomeScreen.Service.MediaClient.Generated;
using HomeScreen.Service.Proto.Services;
using HomeScreen.ServiceDefaults;
using HomeScreen.Web.Slideshow.Server.Services;
using NJsonSchema.Generation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(
    document =>
    {
        document.Description = "Home Dashboard API";
        document.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
        document.SchemaSettings.GenerateExamples = true;
        document.SchemaSettings.GenerateEnumMappingDescription = true;
    }
);
builder.Services.AddTransient(typeof(IJsonStreamingResultExecutor<>), typeof(JsonStreamingResultExecutor<>));

builder.Services.AddGrpcClient<MediaGrpcClient>(
    "homescreen-service-media",
    c => c.Address = new Uri(
        builder.Configuration.GetSection("services")
            .GetSection("homescreen-service-media")
            .GetSection("http")
            .GetChildren()
            .FirstOrDefault()!.Value!
    )
);
builder.Services.AddGrpcClient<WeatherGrpcClient>(
    "homescreen-service-weather",
    c => c.Address = new Uri(
        builder.Configuration.GetSection("services")
            .GetSection("homescreen-service-weather")
            .GetSection("http")
            .GetChildren()
            .FirstOrDefault()!.Value!
    )
);
builder.Services.AddHttpClient(
    "MediaDownloader",
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration.GetSection("services")
                .GetSection("homescreen-service-media")
                .GetSection("http")
                .GetChildren()
                .FirstOrDefault()!.Value!
        );
        client.DefaultRequestVersion = new Version(2, 0);
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
    }
);
builder.Services.AddScoped<IMediaFileClient, MediaFileClient>(
    sp => new MediaFileClient("", sp.GetRequiredService<IHttpClientFactory>().CreateClient("MediaDownloader"))
);
builder.Services.AddScoped<IMediaDownloader, MediaDownloader>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml");
    app.UseSwaggerUi(p => p.DocumentPath = "/swagger/{documentName}/swagger.yaml");
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();
