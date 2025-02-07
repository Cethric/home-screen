using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Azure.Core.GeoJson;
using Azure.Maps;
using Azure.Maps.Search;
using Azure.Maps.Search.Models;

namespace HomeScreen.Service.Location.Infrastructure.Azure;

public class AzureMapsSearchApi(ILogger<AzureMapsSearchApi> logger, MapsSearchClient mapsSearchClient)
    : IAzureMapsSearchApi
{
    private static ActivitySource ActivitySource => new(nameof(AzureLocationApi));

    public async Task<ReverseSearchAddressResponse?> ReverseSearchAddressAsync(
        GeoPosition coordinates,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddTag("options", JsonSerializer.Serialize(coordinates));
        logger.LogTrace("Searching for reverse address {ReverseSearchOptions}", coordinates);
        var response = await mapsSearchClient.GetReverseGeocodingAsync(
            coordinates,
            new ReverseGeocodingQuery
            {
                ResultTypes =
                [
                    ReverseGeocodingResultTypeEnum.Neighborhood, ReverseGeocodingResultTypeEnum.PopulatedPlace
                ],
                LocalizedMapView = LocalizedMapView.Auto,
                Coordinates = coordinates,
                OptionalId = string.Empty
            },
            cancellationToken
        );
        if (response.HasValue)
        {
            activity?.AddEvent(new ActivityEvent("HasValue")).AddBaggage("value", response.Value.ToString());
            logger.LogTrace("Reverse address found {ReverseSearchOptions} {Response}", coordinates, response.Value);
            return new ReverseSearchAddressResponse
            {
                Addresses = response
                    .Value.Features.Select(feat => new ReverseSearchAddress
                        {
                            AddressLine = feat.Properties.Address.AddressLine ?? string.Empty,
                            Locality = feat.Properties.Address.Locality ?? string.Empty,
                            Neighborhood = feat.Properties.Address.Neighborhood ?? string.Empty,
                            AdminDistricts =
                                feat.Properties.Address.AdminDistricts.Select(dist => dist.Name).ToImmutableList(),
                            PostalCode = feat.Properties.Address.PostalCode ?? string.Empty,
                            CountryRegion = feat.Properties.Address.CountryRegion.Name ?? string.Empty,
                            FormattedAddress = feat.Properties.Address.FormattedAddress ?? string.Empty,
                            Intersection = feat.Properties.Address.Intersection.DisplayName ?? string.Empty
                        }
                    )
                    .ToList()
            };
        }

        logger.LogTrace("No reverse address found {ReverseSearchOptions}", coordinates);
        activity?.AddEvent(new ActivityEvent("NoValue"));
        return null;
    }
}
