using Azure.Core.GeoJson;
using Azure.Maps;
using Azure.Maps.Search;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeScreen.Service.Media.Infrastructure.Location.Azure;

public class AzureLocationService(
    ILogger<AzureLocationService> logger,
    IDistributedCache distributedCache,
    IAzureMapsSearchService searchService
) : ILocationService
{
    public async Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Attempting to search for address at {Longitude}, {Latitude}, {Altitude}",
            longitude,
            latitude,
            altitude
        );

        var key = $"{longitude:.05f}-{latitude:.05f}-{altitude:.05f}";
        var label = await distributedCache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(label))
        {
            logger.LogInformation(
                "Using cached address for {Longitude}, {Latitude}, {Altitude} - {FormattedAddress}",
                longitude,
                latitude,
                altitude,
                label
            );
            return label;
        }

        var response = await searchService.ReverseSearchAddressAsync(
            new ReverseSearchOptions
            {
                Coordinates = new GeoPosition(longitude, latitude, altitude),
                Language = SearchLanguage.EnglishAustralia,
                RadiusInMeters = 20,
                LocalizedMapView = LocalizedMapView.Auto
            },
            cancellationToken
        );

        if (response is null || response.Addresses.Count == 0) return ILocationService.UnknownLocation;
        var address = response.Addresses.FirstOrDefault();
        if (address is null) return ILocationService.UnknownLocation;

        var formatted = string.Join(
                " ",
                new HashSet<string>
                {
                    address.StreetName,
                    address.MunicipalitySubdivision,
                    address.Municipality,
                    address.CountryTertiarySubdivision,
                    address.CountrySecondarySubdivision,
                    (address.CountrySecondarySubdivision).Contains(address.CountrySubdivision)
                        ? string.Empty
                        : address.CountrySubdivision,
                    address.Country
                }.Where(x => !string.IsNullOrEmpty(x))
            )
            .Trim();
        logger.LogInformation("Found Address {FormattedAddress}", formatted);
        await distributedCache.SetStringAsync(key, formatted, cancellationToken);
        return formatted;
    }
}
