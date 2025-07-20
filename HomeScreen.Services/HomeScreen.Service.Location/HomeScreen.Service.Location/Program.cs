using System.Text.Json.Serialization;
using HomeScreen.Service.Location;
using HomeScreen.Service.Location.Services;
using HomeScreen.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder.AddLocationServices();
builder
    .Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.MapControllers();
app.MapGrpcService<LocationService>();
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
