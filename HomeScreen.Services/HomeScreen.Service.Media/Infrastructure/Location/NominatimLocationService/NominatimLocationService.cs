using HomeScreen.Service.Media.Infrastructure.Location.NominatimLocationService.Generated.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeScreen.Service.Media.Infrastructure.Location.NominatimLocationService;

public class NominatimLocationService(
    ILogger<NominatimLocationService> logger,
    IDistributedCache distributedCache,
    INominatimClient nominatimClient
) : ILocationService
{
    public async Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken = default
    )
{
    logger.LogInformation(
        "Attempting to search for address at {Longitude}, {Latitude}, {Altitude}",
        longitude,
        latitude,
        altitude
    );

    var key = $"{longitude:.05f}-{latitude:.05f}-{altitude:.05f}";
    var label = await distributedCache.GetStringAsync(key, cancellationToken);
    if (!string.IsNullOrEmpty(label))
    {
        logger.LogInformation(
            "Using cached address for {Longitude}, {Latitude}, {Altitude} - {FormattedAddress}",
            longitude,
            latitude,
            altitude,
            label
        );
        return label;
    }

    var response = await nominatimClient.Reverse_phpAsync(
        latitude,
        longitude,
        format: OutputFormat.Jsonv2,
        zoom: 13,
        cancellationToken: cancellationToken
    );
    label = ILocationService.UnknownLocation;
    if (response.StatusCode == StatusCodes.Status200OK)
    {
        logger.LogInformation(
            "Found address for {Longitude}, {Latitude}, {Altitude} - {FormattedAddress}",
            longitude,
            latitude,
            altitude,
            label
        );
        label = response.Result.Display_name ?? ILocationService.UnknownLocation;
    }
    else
    {
        logger.LogWarning(
            "No Address found for {Longitude}, {Latitude}, {Altitude}",
            longitude,
            latitude,
            altitude
        );
    }

    await distributedCache.SetStringAsync(key, label, cancellationToken);
    return label;
}
}
