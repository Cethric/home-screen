using System.Diagnostics;
using HomeScreen.Service.Location.Infrastructure.NominatimLocationService.Generated.Entities;

namespace HomeScreen.Service.Location.Infrastructure.NominatimLocationService;

public class NominatimLocationApi(ILogger<NominatimLocationApi> logger, INominatimClient nominatimClient) : ILocationApi
{
    private static ActivitySource ActivitySource => new(nameof(NominatimLocationApi));

    public async Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogInformation(
            "Attempting to search for address at {Longitude}, {Latitude}, {Altitude}",
            longitude,
            latitude,
            altitude
        );

        var response = await nominatimClient.Reverse_phpAsync(
            latitude,
            longitude,
            OutputFormat.Jsonv2,
            zoom: 15,
            cancellationToken: cancellationToken
        );
        if (response.StatusCode == StatusCodes.Status200OK && !string.IsNullOrWhiteSpace(response.Result.Display_name))
        {
            logger.LogInformation(
                "Found address for {Longitude}, {Latitude}, {Altitude}",
                longitude,
                latitude,
                altitude
            );
            logger.LogInformation("Found Address {FormattedAddress}", response.Result.Display_name);
            activity?.AddEvent(new ActivityEvent("Found Location")).AddBaggage("value", response.Result.Display_name);
            return response.Result.Display_name;
        }

        activity?.AddEvent(new ActivityEvent("Unknown Location"));
        logger.LogWarning("No Address found for {Longitude}, {Latitude}, {Altitude}", longitude, latitude, altitude);

        return ILocationApi.UnknownLocation;
    }
}
