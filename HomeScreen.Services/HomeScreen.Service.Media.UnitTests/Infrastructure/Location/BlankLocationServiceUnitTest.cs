using FluentAssertions;
using HomeScreen.Service.Media.Infrastructure.Location;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeScreen.Service.Media.UnitTests.Infrastructure.Location;

public class BlankLocationServiceUnitTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 90)]
    public async Task GivenBlankLocationService_WhenRequestionLocation_ThenReturnEmptyString(
        double latitude,
        double longitude
    )
    {
        // Arrange
        var logger = new Mock<ILogger<BlankLocationService>>();
        var service = new BlankLocationService(logger.Object);
        // Act
        var result = await service.SearchForLocation(longitude, latitude, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(ILocationService.UnknownLocation);
    }
}
