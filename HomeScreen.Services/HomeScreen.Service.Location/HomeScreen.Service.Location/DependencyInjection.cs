using Azure;
using Azure.Maps.Search;
using HomeScreen.OpenAPI.Nominatim.Api;
using HomeScreen.Service.Location.Configuration;
using HomeScreen.Service.Location.Infrastructure;
using HomeScreen.Service.Location.Infrastructure.Azure;
using HomeScreen.Service.Location.Infrastructure.NominatimLocationService;

namespace HomeScreen.Service.Location;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddLocationServices(this IHostApplicationBuilder builder) =>
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
        builder.Services.AddSingleton<AzureKeyCredential>(sp =>
            new AzureKeyCredential(sp.GetRequiredService<AzureConfig>().MapSecretKey)
        );
        builder.Services.AddSingleton<MapsSearchClient>(sp =>
            new MapsSearchClient(sp.GetRequiredService<AzureKeyCredential>())
        );
        builder.Services.AddScoped<IAzureMapsSearchApi, AzureMapsSearchApi>();
        builder.Services.AddScoped<ILocationApi, AzureLocationApi>();

        return builder;
    }

    private static IHostApplicationBuilder AddBlankLocationService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ILocationApi, BlankLocationApi>();
        return builder;
    }

    private static IHostApplicationBuilder AddNominatimLocationService(this IHostApplicationBuilder builder)
    {
        const string BaseServerUrl = "https://nominatim.geocoding.ai";
        builder.Services.AddHttpClient("Nominatim");
        builder.Services.AddScoped<IDefaultApi, DefaultApi>(sp => new DefaultApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Nominatim"),
                new OpenAPI.Nominatim.Client.Configuration
                {
                    BasePath = BaseServerUrl,
                    UseDefaultCredentials = false,
                    DefaultHeaders = { },
                    Timeout = 100_000,
                    UserAgent = "HomeScreen.Service.Location"
                }
            )
        );
        builder.Services.AddScoped<ILocationApi, NominatimLocationApi>();
        return builder;
    }
}
