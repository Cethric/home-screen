using HomeScreen.Service.Media.Common;
using HomeScreen.Service.Media.Infrastructure.Media;

namespace HomeScreen.Service.Media;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder) =>
        builder.AddMediaCommon().AddMediaServices();

    private static IHostApplicationBuilder AddMediaServices(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<IMediaApi, MediaApi>()
            .AddScoped<IMediaTransformPath, MediaTransformPath>()
            .AddScoped<IMediaTransformer, MediaTransformer>();

        return builder;
    }
}
