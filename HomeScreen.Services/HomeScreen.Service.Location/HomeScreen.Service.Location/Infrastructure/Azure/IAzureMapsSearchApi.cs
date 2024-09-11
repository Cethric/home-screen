using Azure.Core.GeoJson;

namespace HomeScreen.Service.Location.Infrastructure.Azure;

public interface IAzureMapsSearchApi
{
    Task<ReverseSearchAddressResponse?> ReverseSearchAddressAsync(
        GeoPosition coordinates,
        CancellationToken cancellationToken = default
    );
}
