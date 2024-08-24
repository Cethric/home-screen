namespace HomeScreen.Service.Location.Infrastructure.Location;

public class BlankLocationApi(ILogger<BlankLocationApi> logger) : ILocationApi
{
    public Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Using blank location for address at {Longitude}, {Latitude}, {Altitude}",
            longitude,
            latitude,
            altitude
        );
        return Task.FromResult(ILocationApi.UnknownLocation);
    }
}
