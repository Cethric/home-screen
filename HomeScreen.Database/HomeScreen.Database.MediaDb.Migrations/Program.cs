using HomeScreen.Database.MediaDb;
using HomeScreen.Database.MediaDb.Migrations;
using HomeScreen.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults();
builder.AddMediaDb();

var host = builder.Build();

await host.RunAsync();
