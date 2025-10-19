using System.Diagnostics;
using Grpc.Core;
using HomeScreen.Service.Location.Client.Infrastructure.Location;
using HomeScreen.Service.Media.Common.Infrastructure.Cache;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaLocationProcessor(
    ILogger<MediaLocationProcessor> logger,
    ILocationApi locationApi,
    IGenericCache genericCache,
    IMediaMetadataReader mediaMetadataReader
) : IMediaLocationProcessor
{
    private static ActivitySource ActivitySource => new(nameof(MediaLocationProcessor));

    public async Task<(double, double, string?)> ProcessLocation(FileInfo file, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity();

        var gps = await mediaMetadataReader.LoadGps(file, cancellationToken);

        if (gps?.TryGetGeoLocation(out var geoLocation) is not true)
        {
            logger.LogInformation("No GPS Details found for {Path}", file.FullName);
            return (360, 360, "");
        }

        var altitudeRaw = gps.GetRational(GpsDirectory.TagAltitude);
        var bss = gps.GetBoolean(GpsDirectory.TagAltitudeRef);
        var altitude = bss ? -altitudeRaw.ToDouble() : altitudeRaw.ToDouble();
        logger.LogInformation(
            "Found GPS Details {Longitude} {Latitude} {Altitude}",
            geoLocation.Latitude,
            geoLocation.Longitude,
            altitude
        );
        var cachedLocationKey = $"{geoLocation.Longitude:03.04f}-{geoLocation.Latitude:03.04f}-{altitude:03.04f}";
        var cachedLocation = await genericCache.ReadCache<string>(cachedLocationKey, cancellationToken);
        if (string.IsNullOrEmpty(cachedLocation))
        {
            try
            {
                logger.LogDebug("Attempting to geolocate {CacheKey}", cachedLocationKey);
                cachedLocation = await locationApi.SearchForLocation(
                    geoLocation.Longitude,
                    geoLocation.Latitude,
                    altitude,
                    cancellationToken
                );
                if (!string.IsNullOrEmpty(cachedLocation))
                {
                    await genericCache.WriteCache(cachedLocationKey, cachedLocation, cancellationToken);
                }
            }
            catch (RpcException ex)
            {
                logger.LogError(ex, "Failed to geolocate {CacheKey}", cachedLocationKey);
            }
        }

        logger.LogInformation("Found location for {CacheKey} : {Location}", cachedLocationKey, cachedLocation);

        return (geoLocation.Latitude, geoLocation.Longitude, cachedLocation);
    }
}
