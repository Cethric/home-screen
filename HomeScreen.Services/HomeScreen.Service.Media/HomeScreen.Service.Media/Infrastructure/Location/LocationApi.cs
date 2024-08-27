using System.Diagnostics;
using HomeScreen.Service.Location;
using HomeScreen.Service.Location.Proto.Services;

namespace HomeScreen.Service.Media.Infrastructure.Location;

public class LocationApi(ILogger<LocationApi> logger, LocationGrpcClient client) : ILocationApi
{
    private static ActivitySource ActivitySource => new(nameof(LocationApi));

public async Task<string> SearchForLocation(
    double longitude,
    double latitude,
    double altitude,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity("SearchForLocation", ActivityKind.Client);
    logger.LogInformation("Searching for location name at {Longitude}, {Latitude}", longitude, latitude);
    var result = await client.SearchForLocationAsync(
        new SearchForLocationRequest { Longitude = longitude, Latitude = latitude, Altitude = altitude },
        cancellationToken: cancellationToken
    );
    logger.LogInformation(
        "Found location {Longitude}, {Latitude} => {Location}",
        longitude,
        latitude,
        result.Location
    );
    return result.Location;
}
}
