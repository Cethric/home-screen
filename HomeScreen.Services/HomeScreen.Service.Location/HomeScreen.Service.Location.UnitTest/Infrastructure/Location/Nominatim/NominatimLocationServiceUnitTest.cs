using FluentAssertions;
using HomeScreen.Service.Location.Infrastructure;
using HomeScreen.Service.Location.Infrastructure.NominatimLocationService;
using HomeScreen.Service.Location.Infrastructure.NominatimLocationService.Generated.Entities;
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
        var location = "Example Location";
        var logger = new Mock<ILogger<NominatimLocationApi>>();
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
        var service = new NominatimLocationApi(logger.Object, searchService.Object);
        // Act
        var result = await service.SearchForLocation(lon, lat, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(location);
    }
}
