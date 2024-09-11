using System.Diagnostics;
using System.Globalization;
using Azure.Core.GeoJson;

namespace HomeScreen.Service.Location.Infrastructure.Azure;

public class AzureLocationApi(ILogger<AzureLocationApi> logger, IAzureMapsSearchApi searchApi) : ILocationApi
{
    private static ActivitySource ActivitySource => new(nameof(AzureLocationApi));

public async Task<string> SearchForLocation(
    double longitude,
    double latitude,
    double altitude,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("longitude", longitude.ToString(CultureInfo.InvariantCulture))
        .AddBaggage("latitude", latitude.ToString(CultureInfo.InvariantCulture))
        .AddBaggage("altitude", altitude.ToString(CultureInfo.InvariantCulture));
    logger.LogInformation(
        "Attempting to search for address at {Longitude}, {Latitude}, {Altitude}",
        longitude,
        latitude,
        altitude
    );

    var response = await searchApi.ReverseSearchAddressAsync(
        new GeoPosition(longitude, latitude, altitude),
        cancellationToken
    );

    if (response is null || response.Addresses.Count == 0)
    {
        activity?.AddEvent(new ActivityEvent("Unknown Location"));
        return ILocationApi.UnknownLocation;
    }

    var address = response.Addresses.FirstOrDefault();
    if (address is null)
    {
        activity?.AddEvent(new ActivityEvent("Unknown Location"));
        return ILocationApi.UnknownLocation;
    }

    var formatted = string.Join(
            " ",
            new HashSet<string>
            {
                    address.Intersection,
                    address.Locality,
                    address.Neighborhood,
                    string.Join(" ", address.AdminDistricts),
                    address.CountryRegion
            }.Where(x => !string.IsNullOrEmpty(x))
        )
        .Trim();
    logger.LogInformation("Found Address {FormattedAddress}", formatted);
    activity?.AddEvent(new ActivityEvent("Found Location")).AddBaggage("value", formatted);
    return formatted;
}
}
