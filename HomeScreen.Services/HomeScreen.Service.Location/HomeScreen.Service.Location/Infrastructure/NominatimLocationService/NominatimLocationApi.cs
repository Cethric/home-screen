using System.Diagnostics;
using System.Net;
using HomeScreen.OpenAPI.Nominatim.Api;
using HomeScreen.OpenAPI.Nominatim.Model;

namespace HomeScreen.Service.Location.Infrastructure.NominatimLocationService;

public class NominatimLocationApi(ILogger<NominatimLocationApi> logger, IDefaultApi nominatimClient) : ILocationApi
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

        var response = await nominatimClient.ReverseGetWithHttpInfoAsync(
            latitude,
            longitude,
            OutputFormat.Jsonv2,
            zoom: 16,
            cancellationToken: cancellationToken
        );
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = response.Data;
            if (!string.IsNullOrEmpty(content.DisplayName))
            {
                logger.LogInformation(
                    "Found address for {Longitude}, {Latitude}, {Altitude}",
                    longitude,
                    latitude,
                    altitude
                );
                logger.LogInformation("Found Address {FormattedAddress}", content.DisplayName);
                activity?.AddEvent(new ActivityEvent("Found Location")).AddBaggage("value", content.DisplayName);
                return content.DisplayName;
            }
        }

        activity?.AddEvent(new ActivityEvent("Unknown Location"));
        logger.LogWarning("No Address found for {Longitude}, {Latitude}, {Altitude}", longitude, latitude, altitude);

        return ILocationApi.UnknownLocation;
    }
}
