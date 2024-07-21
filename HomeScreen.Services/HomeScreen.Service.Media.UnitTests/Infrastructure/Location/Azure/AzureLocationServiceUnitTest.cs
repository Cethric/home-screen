using System.Text;
using Azure;
using Azure.Maps.Search;
using Azure.Maps.Search.Models;
using FluentAssertions;
using HomeScreen.Service.Media.Infrastructure.Location;
using HomeScreen.Service.Media.Infrastructure.Location.Azure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeScreen.Service.Media.UnitTests.Infrastructure.Location.Azure;

public class AzureLocationServiceUnitTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenAzureLocationService_WhenRequestingLocationFromCache_ThenReturnCachedString(
        double latitude,
        double longitude
    )
    {
        // Arrange
        var value = "Location Name";
        var logger = new Mock<ILogger<AzureLocationService>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(value));
        var searchService = new Mock<IAzureMapsSearchService>();
        var service = new AzureLocationService(logger.Object, cache.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(longitude, latitude, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task
        GivenAzureLocationService_WhenRequestingLocationWithNoCacheAndNoResponse_ThenReturnUnknownLocation(
            double latitude,
            double longitude
        )
    {
        // Arrange
        var logger = new Mock<ILogger<AzureLocationService>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as byte[]);
        var response = new Mock<Response<ReverseSearchAddressResult>>();
        response.SetupGet(x => x.HasValue).Returns(false);
        var searchService = new Mock<IAzureMapsSearchService>();
        searchService
            .Setup(x => x.ReverseSearchAddressAsync(It.IsAny<ReverseSearchOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as ReverseSearchAddressResponse);
        var service = new AzureLocationService(logger.Object, cache.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(longitude, latitude, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(ILocationService.UnknownLocation);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenAzureLocationService_WhenRequestingLocationWithNoCacheAndValidResponse_ThenReturnLocation(
        double latitude,
        double longitude
    )
    {
        // Arrange
        var logger = new Mock<ILogger<AzureLocationService>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as byte[]);
        var searchService = new Mock<IAzureMapsSearchService>();
        searchService
            .Setup(x => x.ReverseSearchAddressAsync(It.IsAny<ReverseSearchOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ReverseSearchAddressResponse
                {
                    Addresses = new List<ReverseSearchAddress>
                    {
                        new()
                        {
                            StreetName = "1",
                            MunicipalitySubdivision = "2",
                            Municipality = "3",
                            CountryTertiarySubdivision = "4",
                            CountrySecondarySubdivision = "5",
                            CountrySubdivision = "6",
                            Country = "7",
                        }
                    }
                }
            );
        var service = new AzureLocationService(logger.Object, cache.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(longitude, latitude, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo("1 2 3 4 5 6 7");
    }
}
