using FluentAssertions;
using HomeScreen.Service.Location.Infrastructure.Location;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeScreen.Service.Location.UnitTest.Infrastructure.Location;

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
        var logger = new Mock<ILogger<BlankLocationApi>>();
        var service = new BlankLocationApi(logger.Object);
        // Act
        var result = await service.SearchForLocation(longitude, latitude, 0, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(ILocationApi.UnknownLocation);
    }
}
