namespace HomeScreen.Service.Media.Infrastructure.Location;

public interface ILocationApi
{
    Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken = default
    );
}
