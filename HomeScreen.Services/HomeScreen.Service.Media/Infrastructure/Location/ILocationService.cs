namespace HomeScreen.Service.Media.Infrastructure.Location;

public interface ILocationService
{
    public const string UnknownLocation = "Unknown location";

    Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken = default
    );
}
