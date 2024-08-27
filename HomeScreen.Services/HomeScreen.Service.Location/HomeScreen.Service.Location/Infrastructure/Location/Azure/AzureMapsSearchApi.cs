using Azure.Maps.Search;

namespace HomeScreen.Service.Location.Infrastructure.Location.Azure;

public class AzureMapsSearchApi(ILogger<AzureMapsSearchApi> logger, MapsSearchClient mapsSearchClient)
    : IAzureMapsSearchApi
{
    public async Task<ReverseSearchAddressResponse?> ReverseSearchAddressAsync(
        ReverseSearchOptions options,
        CancellationToken cancellationToken = default
    )
{
    logger.LogTrace("Searching for reverse address {ReverseSearchOptions}", options);
    var response = await mapsSearchClient.ReverseSearchAddressAsync(options, cancellationToken);
    if (response.HasValue)
    {
        logger.LogTrace("Reverse address found {ReverseSearchOptions} {Response}", options, response.Value);
        return new ReverseSearchAddressResponse
        {
            Addresses = response.Value.Addresses.Select(
                                           address => new ReverseSearchAddress
                                           {
                                               StreetName = address.Address.StreetName ?? string.Empty,
                                               MunicipalitySubdivision =
                                                              address.Address.MunicipalitySubdivision ??
                                                              string.Empty,
                                               Municipality = address.Address.Municipality ??
                                                                         string.Empty,
                                               CountryTertiarySubdivision =
                                                              address.Address.CountryTertiarySubdivision ??
                                                              string.Empty,
                                               CountrySecondarySubdivision =
                                                              address.Address.CountrySecondarySubdivision ??
                                                              string.Empty,
                                               CountrySubdivision = address.Address.CountrySubdivision ??
                                                              string.Empty,
                                               Country = address.Address.Country ?? string.Empty
                                           }
                                       )
                                       .ToList()
        };
    }

    logger.LogTrace("No reverse address found {ReverseSearchOptions}", options);
    return null;
}
}
