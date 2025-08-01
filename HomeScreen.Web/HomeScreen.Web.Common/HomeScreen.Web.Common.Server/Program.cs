using System.Net.Mime;
using System.Text.Json.Serialization;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Service.Media.Proto.Services;
using HomeScreen.Service.Weather.Proto.Services;
using HomeScreen.ServiceDefaults;
using HomeScreen.Web.Common.JsonLines;
using HomeScreen.Web.Common.Server.Endpoints;
using HomeScreen.Web.Common.Server.Entities;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);
builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);

builder.Services.AddEndpointsApiExplorer();
builder
    .Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApiDocument(document =>
    {
        document.Title = "HomeScreen Common API";
        document.Description = "";
        document.Version = GitVersionInformation.FullSemVer;
        document.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
        document.SchemaSettings.GenerateExamples = true;
        document.SchemaSettings.GenerateEnumMappingDescription = true;
        document.SchemaSettings.TypeMappers.Add(
            new ObjectTypeMapper(
                typeof(FileStreamHttpResult),
                new JsonSchema { Type = JsonObjectType.File, Format = "binary" }
            )
        );
        document.OperationProcessors.Add(
            new OperationProcessor(context =>
                {
                    switch (context.MethodInfo.Name)
                    {
                        case "RandomMedia":
                            ProcessRandomMediaMime(context);
                            break;
                        case "DownloadMedia":
                            ProcessFileMime(context);
                            break;
                    }

                    return true;
                }
            )
        );
    }
);
builder.Services.AddTransient(typeof(IJsonLinesExecutor<>), typeof(JsonLinesExecutor<>));
builder.Services.AddGrpcClient<MediaGrpcClient>(
    "homescreen-service-media",
    c => c.Address = new Uri(
        builder
            .Configuration.GetSection("services")
            .GetSection("homescreen-service-media")
            .GetSection("http")
            .GetChildren()
            .FirstOrDefault()!.Value!
    )
);
builder.AddGrpcHealthCheck("homescreen-service-media");

builder.Services.AddGrpcClient<WeatherGrpcClient>(
    "homescreen-service-weather",
    c => c.Address = new Uri(
        builder
            .Configuration.GetSection("services")
            .GetSection("homescreen-service-weather")
            .GetSection("http")
            .GetChildren()
            .FirstOrDefault()!.Value!
    )
);
builder.AddGrpcHealthCheck("homescreen-service-weather");

builder.Services.AddHttpClient(
    "MediaDownloader",
    client =>
    {
        client.BaseAddress = new Uri(
            builder
                .Configuration.GetSection("services")
                .GetSection("homescreen-service-media")
                .GetSection("http")
                .GetChildren()
                .FirstOrDefault()!.Value!
        );
        client.DefaultRequestVersion = new Version(2, 0);
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
    }
);
builder.Services.AddScoped<IMediaClient, MediaClient>(sp => new MediaClient(
        "",
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("MediaDownloader")
    )
);
builder.Services.AddScoped<IMediaApi, MediaApi>();
builder.Services.AddScoped<IWeatherApi, WeatherApi>();
builder.Services.AddScoped<IConfigApi, ConfigApi>();
builder.Services.AddCors();

var app = builder.Build();
app.MapDefaultEndpoints();
app.RegisterConfigEndpoints();
app.RegisterMediaEndpoints();
app.RegisterWeatherEndpoints();
app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.json");
    app.UseSwaggerUi(p => p.DocumentPath = "/swagger/{documentName}/swagger.json");
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app
    .Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();
return;

static void ProcessRandomMediaMime(OperationProcessorContext context)
{
    context.OperationDescription.Operation.Produces.Clear();
    context.OperationDescription.Operation.Produces = [MediaTypeNames.Application.JsonSequence];
    context.OperationDescription.Operation.Responses.Clear();

    var response = new OpenApiResponse
    {
        Schema = new JsonSchema
        {
            Type = JsonObjectType.Object, Reference = context.Document.Definitions["MediaItem"]
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
                    Type = JsonObjectType.Object, Reference = context.Document.Definitions["MediaItem"]
                }
            }
        );

    context.OperationDescription.Operation.Responses.Add("200", response);
}


static void ProcessFileMime(OperationProcessorContext context)
{
    context.OperationDescription.Operation.Produces.Clear();
    context.OperationDescription.Operation.Produces =
    [
        MediaTransformOptionsFormat.Jpeg.TransformFormatToMime(),
        MediaTransformOptionsFormat.JpegXl.TransformFormatToMime(),
        MediaTransformOptionsFormat.Png.TransformFormatToMime(),
        MediaTransformOptionsFormat.WebP.TransformFormatToMime(),
        MediaTransformOptionsFormat.Avif.TransformFormatToMime()
    ];
    context.OperationDescription.Operation.Responses.Clear();

    var response = new OpenApiResponse
    {
        Schema = new JsonSchema
        {
            Type = JsonObjectType.File, Reference = context.Document.Definitions["FileStreamHttpResult"]
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
                    Type = JsonObjectType.File, Reference = context.Document.Definitions["FileStreamHttpResult"]
                }
            }
        );

    context.OperationDescription.Operation.Responses.Add("200", response);
}
