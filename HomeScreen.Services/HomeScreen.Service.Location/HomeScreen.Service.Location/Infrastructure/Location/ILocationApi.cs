namespace HomeScreen.Service.Location.Infrastructure.Location;

public interface ILocationApi
{
    public const string UnknownLocation = "Unknown location";

    Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken = default
    );
}
