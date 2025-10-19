using System.Net;
using FluentAssertions;
using HomeScreen.OpenAPI.Nominatim.Api;
using HomeScreen.OpenAPI.Nominatim.Client;
using HomeScreen.OpenAPI.Nominatim.Model;
using HomeScreen.Service.Location.Infrastructure;
using HomeScreen.Service.Location.Infrastructure.NominatimLocationService;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeScreen.Service.Location.UnitTest.Infrastructure.Location.Nominatim;

public class NominatimLocationServiceUnitTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenNominatimLocationService_WhenRequestingLocationWithInvalidResponse_ThenReturnUnknownLocation(
        double lat,
        double lon
    )
    {
        // Arrange
        var logger = new Mock<ILogger<NominatimLocationApi>>();
        var searchService = new Mock<IDefaultApi>();
        searchService
            .Setup(x => x.ReverseGetWithHttpInfoAsync(
                    lat,
                    lon,
                    It.IsAny<OutputFormat>(),
                    It.IsAny<string?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<Layer?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<float?>(),
                    It.IsAny<string?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ApiResponse<ReverseOutputJson>(
                    HttpStatusCode.NotFound,
                    new ReverseOutputJson(),
                    "{}"
                )
            );
        var service = new NominatimLocationApi(logger.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(lon, lat, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(ILocationApi.UnknownLocation);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenNominatimLocationService_WhenRequestingLocationWithValidResponse_ThenReturnLocation(
        double lat,
        double lon
    )
    {
        // Arrange
        var location = $"Location {lat} {lon}";
        var logger = new Mock<ILogger<NominatimLocationApi>>();
        var searchService = new Mock<IDefaultApi>();
        searchService
            .Setup(x => x.ReverseGetWithHttpInfoAsync(
                    lat,
                    lon,
                    It.IsAny<OutputFormat>(),
                    It.IsAny<string?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<NumberBoolean?>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<Layer?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<float?>(),
                    It.IsAny<string?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ApiResponse<ReverseOutputJson>(
                    HttpStatusCode.OK,
                    new ReverseOutputJson(displayName: $"Location {lat} {lon}"),
                    "{}"
                )
            );
        var service = new NominatimLocationApi(logger.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(lon, lat, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(location);
    }
}
