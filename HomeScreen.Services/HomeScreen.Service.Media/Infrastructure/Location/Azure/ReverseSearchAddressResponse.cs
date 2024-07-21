namespace HomeScreen.Service.Media.Infrastructure.Location.Azure;

public class ReverseSearchAddressResponse
{
    public IReadOnlyList<ReverseSearchAddress> Addresses { get; init; } = new List<ReverseSearchAddress>();
}
