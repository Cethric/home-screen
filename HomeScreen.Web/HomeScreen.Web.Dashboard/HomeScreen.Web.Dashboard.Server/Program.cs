using HomeScreen.ServiceDefaults;
using HomeScreen.Web.Dashboard.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults(GitVersionInformation.InformationalVersion);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapDefaultEndpoints();
app.RegisterConfigEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Launching version: {Version}", GitVersionInformation.InformationalVersion);
await app.RunAsync();
