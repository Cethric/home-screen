using System.Diagnostics;

namespace HomeScreen.Service.Location.Infrastructure;

public class BlankLocationApi(ILogger<BlankLocationApi> logger) : ILocationApi
{
    private static ActivitySource ActivitySource => new(nameof(BlankLocationApi));

public Task<string> SearchForLocation(
    double longitude,
    double latitude,
    double altitude,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogWarning(
        "Using blank location for address at {Longitude}, {Latitude}, {Altitude}",
        longitude,
        latitude,
        altitude
    );
    return Task.FromResult(ILocationApi.UnknownLocation);
}
}
