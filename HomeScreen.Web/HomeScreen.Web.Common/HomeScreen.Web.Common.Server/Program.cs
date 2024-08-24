using HomeScreen.Service.Media.Proto.Services;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Service.Weather.Proto.Services;
using HomeScreen.ServiceDefaults;
using HomeScreen.Web.Common;
using HomeScreen.Web.Common.Server.Endpoints;
using HomeScreen.Web.Common.Server.Services;
using NJsonSchema.Generation;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(
    document =>
    {
        document.Description = "HomeScreen Common API";
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
builder.Services.AddScoped<IMediaApi, MediaApi>();
builder.Services.AddScoped<IWeatherApi, WeatherApi>();
builder.Services.AddCors();

var app = builder.Build();
app.RegisterConfigEndpoints();
app.RegisterMediaEndpoints();
app.RegisterWeatherEndpoints();
app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml");
    app.UseSwaggerUi(p => p.DocumentPath = "/swagger/{documentName}/swagger.yaml");
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.Services.GetRequiredService<ILogger<Program>>()
   .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();
