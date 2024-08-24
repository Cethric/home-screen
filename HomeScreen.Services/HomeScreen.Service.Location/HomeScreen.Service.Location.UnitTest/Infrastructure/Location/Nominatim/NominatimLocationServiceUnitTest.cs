using System.Text;
using FluentAssertions;
using HomeScreen.Service.Location.Infrastructure.Location;
using HomeScreen.Service.Location.Infrastructure.Location.NominatimLocationService;
using HomeScreen.Service.Location.Infrastructure.Location.NominatimLocationService.Generated.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeScreen.Service.Location.UnitTest.Infrastructure.Location.Nominatim;

public class NominatimLocationServiceUnitTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenNominatimLocationService_WhenRequestingLocationFromCache_ThenReturnCachedString(
        double lat,
        double lon
    )
    {
        // Arrange
        var value = "Location Name";
        var logger = new Mock<ILogger<NominatimLocationApi>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(Encoding.UTF8.GetBytes(value));
        var searchService = new Mock<INominatimClient>();
        var service = new NominatimLocationApi(logger.Object, cache.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(lon, lat, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task
        GivenNominatimLocationService_WhenRequestingLocationWithNoCacheAndInvalidResponse_ThenReturnUnknownLocation(
            double lat,
            double lon
        )
    {
        // Arrange
        var logger = new Mock<ILogger<NominatimLocationApi>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as byte[]);
        var searchService = new Mock<INominatimClient>();
        searchService.Setup(
                         x => x.Reverse_phpAsync(
                             lat,
                             lon,
                             It.IsAny<OutputFormat>(),
                             null,
                             null,
                             null,
                             null,
                             It.IsAny<double>(),
                             null,
                             null,
                             null,
                             null,
                             null,
                             null,
                             null,
                             null,
                             It.IsAny<CancellationToken>()
                         )
                     )
                     .ReturnsAsync(
                         new SwaggerResponse<ReverseOutputJson>(
                             404,
                             new Dictionary<string, IEnumerable<string>>(),
                             ReverseOutputJson.FromJson("{}")
                         )
                     );
        var service = new NominatimLocationApi(logger.Object, cache.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(lon, lat, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(ILocationApi.UnknownLocation);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task
        GivenNominatimLocationService_WhenRequestingLocationWithNoCacheAndValidResponse_ThenReturnLocation(
            double lat,
            double lon
        )
    {
        // Arrange
        var location = "Example Location";
        var logger = new Mock<ILogger<NominatimLocationApi>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as byte[]);
        var searchService = new Mock<INominatimClient>();
        searchService.Setup(
                         x => x.Reverse_phpAsync(
                             lat,
                             lon,
                             It.IsAny<OutputFormat>(),
                             null,
                             null,
                             null,
                             null,
                             It.IsAny<double>(),
                             null,
                             null,
                             null,
                             null,
                             null,
                             null,
                             null,
                             null,
                             It.IsAny<CancellationToken>()
                         )
                     )
                     .ReturnsAsync(
                         new SwaggerResponse<ReverseOutputJson>(
                             200,
                             new Dictionary<string, IEnumerable<string>>(),
                             ReverseOutputJson.FromJson($"{{\"display_name\": \"{location}\"}}")
                         )
                     );
        var service = new NominatimLocationApi(logger.Object, cache.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(lon, lat, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(location);
    }
}
