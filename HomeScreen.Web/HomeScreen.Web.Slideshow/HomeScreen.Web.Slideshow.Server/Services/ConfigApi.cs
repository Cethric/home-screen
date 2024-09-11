using System.Diagnostics;
using HomeScreen.Web.Slideshow.Server.Entities;

namespace HomeScreen.Web.Slideshow.Server.Services;

public class ConfigApi(IWebHostEnvironment environment, IConfiguration configuration) : IConfigApi
{
    private static ActivitySource ActivitySource => new(nameof(ConfigApi));

public Task<Config?> ResolveConfig(CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity();

    var commonUrl = GetCommonUrl();
    return string.IsNullOrEmpty(commonUrl)
        ? Task.FromResult<Config?>(null)
        : Task.FromResult<Config?>(new Config { CommonUrl = commonUrl });
}

private string GetCommonUrl()
{
    using var activity = ActivitySource.StartActivity();
    return (environment.IsProduction()
               ? configuration.GetValue<string>("CommonAddress")
               : configuration.GetSection("services")
                   .GetSection("homescreen-web-common-server")
                   .GetSection("http")
                   .GetChildren()
                   .FirstOrDefault()
                   ?.Value) ??
           string.Empty;
}
}
