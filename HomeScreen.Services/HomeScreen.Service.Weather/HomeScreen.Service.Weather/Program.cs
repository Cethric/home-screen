using System.Text.Json.Serialization;
using HomeScreen.Service.Weather;
using HomeScreen.Service.Weather.Generated.Clients;
using HomeScreen.Service.Weather.Generated.Entities;
using HomeScreen.Service.Weather.Services;
using HomeScreen.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder
    .Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

builder.Services.AddHttpClient("OpenMeteoClient");
builder.Services.AddScoped<IOpenMeteoClient, OpenMeteoClient>(sp =>
    new OpenMeteoClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("OpenMeteoClient"))
);
builder.Services.AddScoped<IWeatherApi, WeatherApi>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.MapGrpcService<WeatherService>();
app.MapGrpcHealthChecksService();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app
    .Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();