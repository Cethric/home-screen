using Azure.Core.GeoJson;
using Azure.Maps;
using Azure.Maps.Search;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeScreen.Service.Media.Infrastructure.Location;

public class LocationService(
    ILogger<LocationService> logger,
    IDistributedCache distributedCache,
    MapsSearchClient mapsSearchClient
) : ILocationService
{
    public async Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken
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

        var response = await mapsSearchClient.ReverseSearchAddressAsync(
            new ReverseSearchOptions
            {
                Coordinates = new GeoPosition(longitude, latitude, altitude),
                Language = SearchLanguage.EnglishAustralia,
                RadiusInMeters = 20,
                LocalizedMapView = LocalizedMapView.Auto
            },
            cancellationToken
        );

        if (!response.HasValue && response.Value.Addresses.Count < 1) return "Unknown location";
        var address = response.Value.Addresses[0];

        var formatted = string.Join(
                " ",
                new HashSet<string>
                {
                    address.Address.StreetName ?? string.Empty,
                    address.Address.MunicipalitySubdivision ?? string.Empty,
                    address.Address.Municipality ?? string.Empty,
                    address.Address.CountryTertiarySubdivision ?? string.Empty,
                    address.Address.CountrySecondarySubdivision ?? string.Empty,
                    (address.Address.CountrySecondarySubdivision ?? string.Empty).Contains(
                        address.Address.CountrySubdivision ?? string.Empty
                    )
                        ? string.Empty
                        : address.Address.CountrySubdivision ?? string.Empty,
                    address.Address.Country ?? string.Empty
                }.Where(x => !string.IsNullOrEmpty(x))
            )
            .Trim();
        logger.LogInformation("Found Address {FormattedAddress}", formatted);
        await distributedCache.SetStringAsync(key, formatted, cancellationToken);
        return formatted;
    }
}
