using Azure;
using Azure.Maps.Search;
using HomeScreen.Service.Media.Configuration;
using HomeScreen.Service.Media.Infrastructure.Location;
using HomeScreen.Service.Media.Infrastructure.Media;

namespace HomeScreen.Service.Media;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services.AddMediaServices().AddLocationServices();

    private static IServiceCollection AddMediaServices(this IServiceCollection services)
    {
        services.AddScoped<IMediaApi, MediaApi>();
        services.AddScoped<IMediaHasher, MediaHasher>();
        services.AddScoped<IMediaPaths, MediaPaths>();
        services.AddScoped<IMediaProcessor, MediaProcessor>();

        return services;
    }

    private static IServiceCollection AddLocationServices(this IServiceCollection services)
    {
        services.AddSingleton<AzureKeyCredential>(
            (sp) => new AzureKeyCredential(sp.GetRequiredService<AzureConfig>().MapSecretKey)
        );
        services.AddSingleton<MapsSearchClient>(
            (sp) => new MapsSearchClient(sp.GetRequiredService<AzureKeyCredential>())
        );
        services.AddScoped<ILocationService, LocationService>();

        return services;
    }
}
