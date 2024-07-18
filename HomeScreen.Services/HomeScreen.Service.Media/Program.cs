using System.Text.Json.Serialization;
using HomeScreen.Database.MediaDb;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Configuration;
using HomeScreen.Service.Media.Services;
using HomeScreen.ServiceDefaults;
using NJsonSchema.Generation;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder.AddMediaDb();
builder.AddInfrastructure();

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

// Add services to the container.
builder.Services.AddGrpc(options => { options.EnableDetailedErrors = true; });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApiDocument(
    document =>
    {
        document.Description = "Home Screen Media API";
        document.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
        document.SchemaSettings.GenerateExamples = true;
        document.SchemaSettings.GenerateEnumMappingDescription = true;
    }
);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml");
    app.UseSwaggerUi(p => p.DocumentPath = "/swagger/{documentName}/swagger.yaml");
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.MapControllers();
app.MapGrpcService<MediaService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();
