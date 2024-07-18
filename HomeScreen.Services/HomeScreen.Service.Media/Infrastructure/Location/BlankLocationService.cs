namespace HomeScreen.Service.Media.Infrastructure.Location;

public class BlankLocationService(ILogger<BlankLocationService> logger) : ILocationService
{
    public Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Attempting to search for address at {Longitude}, {Latitude}, {Altitude}",
            longitude,
            latitude,
            altitude
        );
        return Task.FromResult(string.Empty);
    }
}
