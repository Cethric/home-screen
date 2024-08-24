using HomeScreen.Service.Location;
using HomeScreen.Service.Location.Services;
using HomeScreen.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);
builder.AddLocationServices();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<LocationService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

await app.RunAsync();
