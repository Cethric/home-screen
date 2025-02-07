using System.Text;
using FluentAssertions;
using Grpc.Core;
using HomeScreen.Service.Location.Infrastructure;
using HomeScreen.Service.Location.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeScreen.Service.Location.UnitTest.Services;

public class LocationServiceUnitTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenLocationService_WhenRequestingLocationFromCache_ThenReturnCachedString(
        double lat,
        double lon
    )
    {
        // Arrange
        var value = "Location Name";
        var logger = new Mock<ILogger<LocationService>>();
        var cache = new Mock<IDistributedCache>();
        cache
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(value));
        var context = new Mock<ServerCallContext>();
        var service = new LocationService(
            logger.Object,
            new TestLocationApi(ILocationApi.UnknownLocation),
            cache.Object
        );
        // Act
        var result = await service.SearchForLocation(
            new SearchForLocationRequest { Altitude = 0, Latitude = lat, Longitude = lon },
            context.Object
        );
        // Assert
        result.Location.Should().BeEquivalentTo(value);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenLocationService_WhenRequestingLocationWithoutCache_ThenReturnQueriedString(
        double lat,
        double lon
    )
    {
        // Arrange
        var value = "Location Name";
        var logger = new Mock<ILogger<LocationService>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as byte[]);
        var context = new Mock<ServerCallContext>();
        var service = new LocationService(logger.Object, new TestLocationApi(value), cache.Object);
        // Act
        var result = await service.SearchForLocation(
            new SearchForLocationRequest { Altitude = 0, Latitude = lat, Longitude = lon },
            context.Object
        );
        // Assert
        result.Location.Should().BeEquivalentTo(value);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenLocationService_WhenRequestingLocationWithoutCacheOrResponse_ThenReturnUnknownLocation(
        double lat,
        double lon
    )
    {
        // Arrange
        var value = ILocationApi.UnknownLocation;
        var logger = new Mock<ILogger<LocationService>>();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as byte[]);
        var context = new Mock<ServerCallContext>();
        var service = new LocationService(logger.Object, new TestLocationApi(value), cache.Object);
        // Act
        var result = await service.SearchForLocation(
            new SearchForLocationRequest { Altitude = 0, Latitude = lat, Longitude = lon },
            context.Object
        );
        // Assert
        result.Location.Should().BeEquivalentTo(value);
    }

    private class TestLocationApi(string result) : ILocationApi
    {
        public Task<string> SearchForLocation(
            double longitude,
            double latitude,
            double altitude,
            CancellationToken cancellationToken = default
        ) =>
            Task.FromResult(result);
}
}
