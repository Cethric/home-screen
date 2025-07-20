using HomeScreen.Service.Media.Common.Configuration;
using HomeScreen.Service.Media.Common.Infrastructure.Cache;
using HomeScreen.Service.Media.Common.Infrastructure.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeScreen.Service.Media.Common;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddMediaCommon(this IHostApplicationBuilder builder)
    {
        builder
            .AddCache()
            .Services.AddSingleton(
                new MediaDirectories
                {
                    MediaSourceDir =
                        builder.Configuration.GetValue<string>("MediaSourceDir") ??
                        Path.Combine(Path.GetTempPath(), "DashHome", "Source"),
                    MediaCacheDir =
                        builder.Configuration.GetValue<string>("MediaCacheDir") ??
                        Path.Combine(Path.GetTempPath(), "DashHome", "Cache")
                }
            )
            .AddScoped<IMediaPaths, MediaPaths>();

        return builder;
    }

    public static IHostApplicationBuilder AddCache(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IGenericCache, GenericCache>();
        return builder;
    }
}
