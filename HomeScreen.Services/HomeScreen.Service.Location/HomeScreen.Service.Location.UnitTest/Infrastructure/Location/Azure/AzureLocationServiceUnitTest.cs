using Azure;
using Azure.Core.GeoJson;
using Azure.Maps.Search.Models;
using FluentAssertions;
using HomeScreen.Service.Location.Infrastructure;
using HomeScreen.Service.Location.Infrastructure.Azure;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeScreen.Service.Location.UnitTest.Infrastructure.Location.Azure;

public class AzureLocationServiceUnitTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenAzureLocationService_WhenRequestingLocationWithNoResponse_ThenReturnUnknownLocation(
        double latitude,
        double longitude
    )
    {
        // Arrange
        var logger = new Mock<ILogger<AzureLocationApi>>();
        var response = new Mock<Response<GeocodingResponse>>();
        response.SetupGet(x => x.HasValue).Returns(false);
        var searchService = new Mock<IAzureMapsSearchApi>();
        searchService
            .Setup(x => x.ReverseSearchAddressAsync(It.IsAny<GeoPosition>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as ReverseSearchAddressResponse);
        var service = new AzureLocationApi(logger.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(longitude, latitude, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(ILocationApi.UnknownLocation);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenAzureLocationService_WhenRequestingLocationWithValidResponse_ThenReturnLocation(
        double latitude,
        double longitude
    )
    {
        // Arrange
        var logger = new Mock<ILogger<AzureLocationApi>>();
        var searchService = new Mock<IAzureMapsSearchApi>();
        searchService
            .Setup(x => x.ReverseSearchAddressAsync(It.IsAny<GeoPosition>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ReverseSearchAddressResponse
                {
                    Addresses = new List<ReverseSearchAddress>
                    {
                        new()
                        {
                            AddressLine = "1",
                            Locality = "2",
                            Neighborhood = "3",
                            AdminDistricts = ["4", "5"],
                            PostalCode = "6",
                            CountryRegion = "7",
                            FormattedAddress = "8",
                            Intersection = "9"
                        }
                    }
                }
            );
        var service = new AzureLocationApi(logger.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(longitude, latitude, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo("9 2 3 4 5 7");
    }
}
