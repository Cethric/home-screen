using System.Diagnostics;
using Grpc.Core;
using HomeScreen.Service.Location.Infrastructure.Location;

namespace HomeScreen.Service.Location.Services;

public class LocationService(ILogger<LocationService> logger, ILocationApi locationApi) : Location.LocationBase
{
    private static ActivitySource ActivitySource => new(nameof(LocationService));

public override async Task<SearchForLocationResponse> SearchForLocation(
    SearchForLocationRequest request,
    ServerCallContext context
)
{
    using var activity = ActivitySource.StartActivity("SearchForLocation", ActivityKind.Client);
    activity?.AddBaggage("request", request.ToString());
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
    activity?.AddEvent(new ActivityEvent("Location Found")).AddBaggage("value", result);
    return new SearchForLocationResponse { Location = result };
}
}
