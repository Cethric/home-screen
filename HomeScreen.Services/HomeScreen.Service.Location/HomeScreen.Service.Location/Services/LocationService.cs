using System.Diagnostics;
using Grpc.Core;
using HomeScreen.Service.Location.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeScreen.Service.Location.Services;

public class LocationService(
    ILogger<LocationService> logger,
    ILocationApi locationApi,
    IDistributedCache distributedCache
) : Location.LocationBase
{
    private static ActivitySource ActivitySource => new(nameof(LocationService));

    public override async Task<SearchForLocationResponse> SearchForLocation(
        SearchForLocationRequest request,
        ServerCallContext context
    )
    {
        using var activity = ActivitySource.StartActivity("SearchForLocation", ActivityKind.Client);
        activity?.AddBaggage("request", request.ToString());
        logger.LogDebug("Searching for location {Latitude}, {Longitude}", request.Latitude, request.Longitude);

        var key = $"location-{request.Longitude:.03f}-{request.Latitude:.03f}";
        var label = await distributedCache.GetStringAsync(key, context.CancellationToken);
        if (!string.IsNullOrEmpty(label))
        {
            activity?.AddEvent(new ActivityEvent("Cached Location")).AddBaggage("value", label);
            logger.LogDebug(
                "Using cached address for {Longitude}, {Latitude}, {Altitude} - {FormattedAddress}",
                request.Longitude,
                request.Latitude,
                request.Altitude,
                label
            );
            return new SearchForLocationResponse { Location = label };
        }

        var result = await locationApi.SearchForLocation(
            request.Longitude,
            request.Latitude,
            request.Altitude,
            context.CancellationToken
        );
        await distributedCache.SetStringAsync(key, result, context.CancellationToken);
        if (string.Equals(result, ILocationApi.UnknownLocation, StringComparison.OrdinalIgnoreCase))
        {
            activity?.AddEvent(new ActivityEvent("Unknown Location"));
            logger.LogDebug(
                "Unknown location {Latitude}, {Longitude}, {Altitude}",
                request.Latitude,
                request.Longitude,
                request.Altitude
            );
        }
        else
        {
            activity?.AddEvent(new ActivityEvent("Location Found")).AddBaggage("value", result);
            logger.LogDebug(
                "Found location {Latitude}, {Longitude}, {Altitude} => {Location}",
                request.Latitude,
                request.Longitude,
                request.Altitude,
                result
            );
        }

        return new SearchForLocationResponse { Location = result };
    }
}
