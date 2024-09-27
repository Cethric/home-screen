using System.Diagnostics;
using HomeScreen.Database.MediaDb.Entities;
using HomeScreen.Service.Media.Infrastructure.Location;
using ImageMagick;
using ImageMagick.Colors;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaProcessor(
    ILogger<MediaProcessor> logger,
    IMediaPaths mediaPaths,
    IMediaDateTimeProcessor mediaDateTimeProcessor,
    ILocationApi locationApi
) : IMediaProcessor
{
    private static ActivitySource ActivitySource => new(nameof(MediaHasher));

    public async Task<Database.MediaDb.Entities.MediaEntry> ProcessMediaEntry(
        FileInfo file,
        string hash,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        try
        {
            logger.LogDebug("Attempting to process media entry {FileName} - {Hash}", file.Name, hash);
            var @event = activity?.AddEvent(new ActivityEvent("Process Image"));
            var stream = await File.ReadAllBytesAsync(file.FullName, cancellationToken);
            using var image = new MagickImage(stream);
            image.AutoOrient();
            @event?.Stop();
            var histogram = GetBaseImageColour(image);
            var (capturedUtc, capturedOffset) =
                await mediaDateTimeProcessor.MediaCaptureDate(file, hash, cancellationToken);
            var entry = new Database.MediaDb.Entities.MediaEntry(file, hash)
                        {
                            Enabled = true,
                            BaseColourB = histogram[2],
                            BaseColourG = histogram[1],
                            BaseColourR = histogram[0],
                            ImageRatioWidth = image.Width / (double)image.Height,
                            ImageRatioHeight = image.Height / (double)image.Width,
                            CapturedUtc = capturedUtc,
                            CapturedOffset = capturedOffset
                        };

            var profile = image.GetExifProfile();
            if (profile is not null)
            {
                await ProcessLocation(profile, entry, cancellationToken);

                entry.Notes = profile.GetValue(ExifTag.ImageDescription)?.Value ?? "";
            }


            var directory = mediaPaths.GetTransformDirectory(entry.OriginalHash);
            var originalInfo = new FileInfo(Path.Combine(directory.FullName, "original" + file.Extension));
            if (originalInfo.Exists)
            {
                return entry;
            }

            logger.LogDebug("Writing Original File {OriginalFile} {CachedFile}", file.FullName, originalInfo.FullName);
            if (!originalInfo.Exists)
            {
                file.CopyTo(originalInfo.FullName);
            }

            return entry;
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "Unable to process file - IOException {FileName}", file.FullName);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unable to process file - AccessException {FileName}", file.FullName);
            throw;
        }
        catch (MagickMissingDelegateErrorException ex)
        {
            logger.LogError(ex, "Unable to process file - MissingDelegateException {FileName}", file.FullName);
            return new Database.MediaDb.Entities.MediaEntry(file, hash)
                   {
                       Notes = $"No magick handler provided {file.FullName}"
                   };
        }
        catch (MagickCorruptImageErrorException ex)
        {
            logger.LogError(ex, "Unable to process file - CorruptImageException {FileName}", file.FullName);
            return new Database.MediaDb.Entities.MediaEntry(file, hash)
                   {
                       Notes = $"Corrupt image provided {file.FullName}"
                   };
        }
    }

    private async Task ProcessLocation(
        IExifProfile profile,
        Database.MediaDb.Entities.MediaEntry entry,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity();
        if (profile.GetValue(ExifTag.GPSLongitude) == null)
        {
            return;
        }

        var lonRef = profile.GetValue(ExifTag.GPSLongitudeRef)?.Value;
        entry.LongitudeDirection = GpsLongitudeRefToLongitudeDirection(lonRef);
        var latRef = profile.GetValue(ExifTag.GPSLatitudeRef)?.Value;
        entry.LatitudeDirection = GpsLatitudeRefToLatitudeDirection(latRef);
        if (profile.GetValue(ExifTag.GPSLongitude)?.Value is { } longitude)
        {
            var longitudeDegrees = longitude[0].ToDouble();
            var longitudeMinutes = longitude[1].ToDouble();
            var longitudeSeconds = longitude[2].ToDouble();

            entry.Longitude = longitudeDegrees + longitudeMinutes / 60 + longitudeSeconds / 3600;
            entry.Longitude = entry.LongitudeDirection == LongitudeDirection.East ? entry.Longitude : -entry.Longitude;
        }

        if (profile.GetValue(ExifTag.GPSLatitude)?.Value is { } latitude)
        {
            var latitudeDegrees = latitude[0].ToDouble();
            var latitudeMinutes = latitude[1].ToDouble();
            var latitudeSeconds = latitude[2].ToDouble();

            entry.Latitude = latitudeDegrees + latitudeMinutes / 60 + latitudeSeconds / 3600;
            entry.Latitude = entry.LatitudeDirection == LatitudeDirection.North ? entry.Latitude : -entry.Latitude;
        }

        var altitude = profile.GetValue(ExifTag.GPSAltitude)?.Value.ToDouble() ?? 0d;
        var aboveSea = profile.GetValue(ExifTag.GPSAltitudeRef)?.Value == 0;

        entry.LocationLabel = await locationApi.SearchForLocation(
            entry.Longitude,
            entry.Latitude,
            aboveSea ? altitude : -altitude,
            cancellationToken
        );
    }

    private static LongitudeDirection GpsLongitudeRefToLongitudeDirection(string? lonRef)
    {
        using var activity = ActivitySource.StartActivity();
        return lonRef switch
        {
            "W" => LongitudeDirection.West,
            "E" => LongitudeDirection.East,
            _ => throw new ArgumentOutOfRangeException(
                nameof(lonRef),
                lonRef,
                "Invalid longitude reference value provided"
            )
        };
    }

    private static LatitudeDirection GpsLatitudeRefToLatitudeDirection(string? latRef)
    {
        using var activity = ActivitySource.StartActivity();
        return latRef switch
        {
            "N" => LatitudeDirection.North,
            "S" => LatitudeDirection.South,
            _ => throw new ArgumentOutOfRangeException(
                nameof(latRef),
                latRef,
                "Invalid latitude reference value provided"
            )
        };
    }

    private static byte[] GetBaseImageColour(MagickImage image)
    {
        using var activity = ActivitySource.StartActivity();
        image.FilterType = FilterType.Point;
        image.Alpha(AlphaOption.Remove);
        image.Thumbnail(32, 32);
        image.MedianFilter(4);
        var histogram = image.Histogram()
            .OrderByDescending(entry => entry.Value)
            .FirstOrDefault(entry => !entry.Key.IsCmyk);
        return ColorRGB.FromMagickColor(histogram.Key)?.ToMagickColor().ToByteArray() ?? [0, 0, 0, 0];
    }
}
