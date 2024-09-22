using System.Diagnostics;
using HomeScreen.Web.Dashboard.Server.Entities;

namespace HomeScreen.Web.Dashboard.Server.Services;

public class ConfigApi(IWebHostEnvironment environment, IConfiguration configuration) : IConfigApi
{
    private static ActivitySource ActivitySource => new(nameof(ConfigApi));

public Task<Config?> ResolveConfig(CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity();

    var commonUrl = GetCommonUrl();
    var slideshowUrl = GetSlideshowUrl();
    return string.IsNullOrEmpty(commonUrl)
        ? Task.FromResult<Config?>(null)
        : Task.FromResult<Config?>(new Config { CommonUrl = commonUrl, SlideshowUrl = slideshowUrl });
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

private string GetSlideshowUrl()
{
    using var activity = ActivitySource.StartActivity();
    return (environment.IsProduction()
               ? configuration.GetValue<string>("SlideshowAddress")
               : configuration.GetSection("services")
                   .GetSection("homescreen-web-slideshow-server")
                   .GetSection("http")
                   .GetChildren()
                   .FirstOrDefault()
                   ?.Value) ??
           string.Empty;
}
}
