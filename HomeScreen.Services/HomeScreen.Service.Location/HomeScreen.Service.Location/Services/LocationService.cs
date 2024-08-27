using Grpc.Core;
using HomeScreen.Service.Location.Infrastructure.Location;

namespace HomeScreen.Service.Location.Services;

public class LocationService(ILogger<LocationService> logger, ILocationApi locationApi) : Location.LocationBase
{
    public override async Task<SearchForLocationResponse> SearchForLocation(
        SearchForLocationRequest request,
        ServerCallContext context
    )
{
    logger.LogInformation("Searching for location {Latitude}, {Longitude}", request.Latitude, request.Longitude);
    var result = await locationApi.SearchForLocation(
        request.Longitude,
        request.Latitude,
        request.Altitude,
        context.CancellationToken
    );
    logger.LogInformation(
        "Found location {Latitude}, {Longitude} => {Location}",
        request.Latitude,
        request.Longitude,
        result
    );
    return new SearchForLocationResponse { Location = result };
}
}
