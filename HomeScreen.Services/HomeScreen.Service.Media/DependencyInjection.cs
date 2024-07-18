using Azure;
using Azure.Maps.Search;
using HomeScreen.Service.Media.Configuration;
using HomeScreen.Service.Media.Infrastructure.Location;
using HomeScreen.Service.Media.Infrastructure.Media;

namespace HomeScreen.Service.Media;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder) =>
        builder.AddMediaServices().AddLocationServices();

    private static IHostApplicationBuilder AddMediaServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMediaApi, MediaApi>();
        builder.Services.AddScoped<IMediaHasher, MediaHasher>();
        builder.Services.AddScoped<IMediaPaths, MediaPaths>();
        builder.Services.AddScoped<IMediaProcessor, MediaProcessor>();
        builder.Services.AddScoped<IMediaTransformer, MediaTransformer>();

        return builder;
    }

    private static IHostApplicationBuilder AddLocationServices(this IHostApplicationBuilder builder) =>
        builder.AddLocationService(builder.Configuration.GetValue<MappingService>("MappingService"));

    private static IHostApplicationBuilder AddLocationService(
        this IHostApplicationBuilder builder,
        MappingService service
    ) =>
        service switch
        {
            MappingService.AzureMaps => builder.AddAzureLocationService(),
            MappingService.Blank => builder.AddBlankLocationService(),
            MappingService.Nominatim => builder.AddNominatimLocationService(),
            _ => throw new ArgumentOutOfRangeException(nameof(service), service, "Invalid service name provided")
        };

    private static IHostApplicationBuilder AddAzureLocationService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(
            new AzureConfig
            {
                MapSecretKey = builder.Configuration.GetValue<string>("AZURE_MAPS_SUBSCRIPTION_KEY") ?? string.Empty
            }
        );
        builder.Services.AddSingleton<AzureKeyCredential>(
            (sp) => new AzureKeyCredential(sp.GetRequiredService<AzureConfig>().MapSecretKey)
        );
        builder.Services.AddSingleton<MapsSearchClient>(
            (sp) => new MapsSearchClient(sp.GetRequiredService<AzureKeyCredential>())
        );
        builder.Services.AddScoped<ILocationService, AzureLocationService>();

        return builder;
    }

    private static IHostApplicationBuilder AddBlankLocationService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ILocationService, BlankLocationService>();
        return builder;
    }

    private static IHostApplicationBuilder AddNominatimLocationService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient("Nominatim");
        builder.Services.AddScoped<ILocationService, NominatimLocationService>();
        return builder;
    }
}
