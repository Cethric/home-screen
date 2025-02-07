using System.Text.Json.Serialization;
using HomeScreen.Database.MediaDb;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Configuration;
using HomeScreen.Service.Media.Endpoints;
using HomeScreen.Service.Media.Entities;
using HomeScreen.Service.Media.Services;
using HomeScreen.ServiceDefaults;
using Microsoft.AspNetCore.Http.HttpResults;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using NSwag;
using NSwag.Generation.Processors;

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
builder
    .Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);
builder.Services.AddOpenApiDocument(document =>
    {
        document.Title = "Home Screen Media API";
        document.Description = "";
        document.Version = GitVersionInformation.FullSemVer;
        document.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
        document.SchemaSettings.GenerateExamples = true;
        document.SchemaSettings.GenerateEnumMappingDescription = true;
        document.SchemaSettings.TypeMappers.Add(
            new ObjectTypeMapper(
                typeof(FileStreamHttpResult),
                new JsonSchema
                {
                    Type = JsonObjectType.File,
                    Format = "binary",
                    Title = nameof(FileStreamHttpResult),
                    Id = nameof(FileStreamHttpResult)
                }
            )
        );
        document.OperationProcessors.Add(
            new OperationProcessor(context =>
                {
                    if (context.MethodInfo.Name != "DownloadMedia") return true;

                    context.OperationDescription.Operation.Produces.Clear();
                    context.OperationDescription.Operation.Produces =
                    [
                        MediaTransformOptionsFormat.Jpeg.TransformFormatToMime(),
                        MediaTransformOptionsFormat.JpegXl.TransformFormatToMime(),
                        MediaTransformOptionsFormat.Png.TransformFormatToMime(),
                        MediaTransformOptionsFormat.WebP.TransformFormatToMime(),
                        MediaTransformOptionsFormat.Avif.TransformFormatToMime()
                    ];
                    // context.OperationDescription.Operation.Responses.Clear();

                    context.Document.Definitions.Add(
                        nameof(FileStreamHttpResult),
                        new JsonSchema { Type = JsonObjectType.File, Format = "binary" }
                    );

                    var response = new OpenApiResponse
                    {
                        Schema = new JsonSchema
                        {
                            Type = JsonObjectType.File,
                            Reference = context.Document.Definitions[nameof(FileStreamHttpResult)]
                        }
                    };
                    response.Content.Clear();
                    foreach (var operation in context.OperationDescription.Operation.Produces)
                        response.Content.Add(
                            operation,
                            new OpenApiMediaType
                            {
                                Schema = new JsonSchema
                                {
                                    Type = JsonObjectType.File,
                                    Reference = context.Document.Definitions[nameof(FileStreamHttpResult)]
                                }
                            }
                        );

                    context.OperationDescription.Operation.Responses.Add("200", response);

                    return true;
                }
            )
        );
    }
);

var app = builder.Build();

app.RegisterMediaEndpoints();
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.MapControllers();
app.MapGrpcService<MediaService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app
    .Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();
