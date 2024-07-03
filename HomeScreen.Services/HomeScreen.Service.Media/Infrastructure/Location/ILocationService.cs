namespace HomeScreen.Service.Media.Infrastructure.Location;

public interface ILocationService
{
    Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken
    );
}
