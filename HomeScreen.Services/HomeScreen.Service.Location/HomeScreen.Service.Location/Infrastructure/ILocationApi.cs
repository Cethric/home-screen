namespace HomeScreen.Service.Location.Infrastructure;

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
