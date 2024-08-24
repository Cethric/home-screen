using Azure.Maps.Search;

namespace HomeScreen.Service.Location.Infrastructure.Location.Azure;

public interface IAzureMapsSearchApi
{
    Task<ReverseSearchAddressResponse?> ReverseSearchAddressAsync(
        ReverseSearchOptions options,
        CancellationToken cancellationToken = default
    );
}
