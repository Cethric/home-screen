using System.Diagnostics;
using System.Text.Json;
using Azure.Maps.Search;

namespace HomeScreen.Service.Location.Infrastructure.Location.Azure;

public class AzureMapsSearchApi(ILogger<AzureMapsSearchApi> logger, MapsSearchClient mapsSearchClient)
    : IAzureMapsSearchApi
{
    private static ActivitySource ActivitySource => new(nameof(AzureLocationApi));

    public async Task<ReverseSearchAddressResponse?> ReverseSearchAddressAsync(
        ReverseSearchOptions options,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddTag("options", JsonSerializer.Serialize(options));
        logger.LogTrace("Searching for reverse address {ReverseSearchOptions}", options);
        var response = await mapsSearchClient.ReverseSearchAddressAsync(options, cancellationToken);
        if (response.HasValue)
        {
            activity?.AddEvent(new ActivityEvent("HasValue")).AddBaggage("value", response.Value.ToString());
            logger.LogTrace("Reverse address found {ReverseSearchOptions} {Response}", options, response.Value);
            return new ReverseSearchAddressResponse
                   {
                       Addresses = response.Value.Addresses.Select(
                               address => new ReverseSearchAddress
                                          {
                                              StreetName = address.Address.StreetName ?? string.Empty,
                                              MunicipalitySubdivision =
                                                  address.Address.MunicipalitySubdivision ?? string.Empty,
                                              Municipality = address.Address.Municipality ?? string.Empty,
                                              CountryTertiarySubdivision =
                                                  address.Address.CountryTertiarySubdivision ?? string.Empty,
                                              CountrySecondarySubdivision =
                                                  address.Address.CountrySecondarySubdivision ?? string.Empty,
                                              CountrySubdivision = address.Address.CountrySubdivision ?? string.Empty,
                                              Country = address.Address.Country ?? string.Empty
                                          }
                           )
                           .ToList()
                   };
        }

        logger.LogTrace("No reverse address found {ReverseSearchOptions}", options);
        activity?.AddEvent(new ActivityEvent("NoValue"));
        return null;
    }
}
