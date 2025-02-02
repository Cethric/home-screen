using System.Text.Json.Serialization;
using HomeScreen.ServiceDefaults;
using HomeScreen.Web.Slideshow.Server.Endpoints;
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
        document.Title = "HomeScreen Slideshow API";
        document.Description = "";
        document.Version = GitVersionInformation.FullSemVer;
        document.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
        document.SchemaSettings.GenerateExamples = true;
        document.SchemaSettings.GenerateEnumMappingDescription = true;
    }
);
builder.Services.AddTransient<IConfigApi, ConfigApi>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseDefaultFiles();
app.RegisterConfigEndpoints();
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