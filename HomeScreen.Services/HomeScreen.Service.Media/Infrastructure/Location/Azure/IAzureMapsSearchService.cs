using Azure.Maps.Search;

namespace HomeScreen.Service.Media.Infrastructure.Location.Azure;

public interface IAzureMapsSearchService
{
    Task<ReverseSearchAddressResponse?> ReverseSearchAddressAsync(
        ReverseSearchOptions options,
        CancellationToken cancellationToken = default
    );
}
