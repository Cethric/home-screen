using System.Text.Json.Serialization;
using HomeScreen.Service.Proto.Services;
using HomeScreen.ServiceDefaults;
using HomeScreen.Web.Slideshow.Server.Services;
using NJsonSchema.Generation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// builder.Services.AddSwaggerDocument();
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

builder.Services.AddGrpcClient<MediaGrpcClient>(
    "homescreen-service-media",
    c => c.Address = new Uri("http://homescreen-service-media:5014")
);
builder.Services.AddGrpcClient<WeatherGrpcClient>(
    "homescreen-service-weather",
    c => c.Address = new Uri("http://homescreen-service-weather:5016")
);
builder.Services.AddHttpClient(
    "MediaDownloader",
    client => { client.BaseAddress = new Uri("http://homescreen-service-media:5014"); }
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

app.Run();
