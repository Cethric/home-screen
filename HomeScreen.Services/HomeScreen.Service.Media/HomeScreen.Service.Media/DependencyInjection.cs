using HomeScreen.Service.Location.Proto.Services;
using HomeScreen.Service.Media.Infrastructure.Location;
using HomeScreen.Service.Media.Infrastructure.Media;

namespace HomeScreen.Service.Media;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder) =>
        builder.AddMediaServices().AddLocationApi();

    private static IHostApplicationBuilder AddLocationApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ILocationApi, LocationApi>();
        builder.Services.AddGrpcClient<LocationGrpcClient>(
            "homescreen-service-location",
            c => c.Address = new Uri(
                builder.Configuration.GetSection("services")
                    .GetSection("homescreen-service-location")
                    .GetSection("http")
                    .GetChildren()
                    .FirstOrDefault()!.Value!
            )
        );

        return builder;
    }

    private static IHostApplicationBuilder AddMediaServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMediaApi, MediaApi>();
        builder.Services.AddScoped<IMediaHasher, MediaHasher>();
        builder.Services.AddScoped<IMediaPaths, MediaPaths>();
        builder.Services.AddScoped<IMediaProcessor, MediaProcessor>();
        builder.Services.AddScoped<IMediaTransformer, MediaTransformer>();

        return builder;
    }
}
