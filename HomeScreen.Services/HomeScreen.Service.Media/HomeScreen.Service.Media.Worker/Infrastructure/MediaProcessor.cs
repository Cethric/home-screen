using System.Diagnostics;
using System.Text.Json;
using HomeScreen.Database.MediaDb.Entities;
using HomeScreen.Service.Media.Common.Infrastructure.Media;
using ImageMagick;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaProcessor(
    ILogger<MediaProcessor> logger,
    IMediaPaths mediaPaths,
    IMediaColourProcessor mediaColourProcessor,
    IMediaDateTimeProcessor mediaDateTimeProcessor,
    IMediaLocationProcessor mediaLocationProcessor
) : IMediaProcessor
{
    private static ActivitySource ActivitySource => new(nameof(MediaProcessor));

    public async Task<MediaEntry> ProcessMediaEntry(
        FileInfo file,
        string hash,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        try
        {
            logger.LogDebug("Attempting to process media entry {FileName} - {Hash}", file.Name, hash);
            var stream = await File.ReadAllBytesAsync(file.FullName, cancellationToken);
            using var image = new MagickImage(stream);
            image.AutoOrient();

            return await ProcessMedia(image, file, hash, cancellationToken);
        }
        catch (MagickMissingDelegateErrorException ex)
        {
            logger.LogError(ex, "Unable to process file - MissingDelegateException {FileName}", file.FullName);
            return new MediaEntry(file, hash) { Notes = $"No magick handler provided {file.FullName}" };
        }
        catch (MagickCorruptImageErrorException ex)
        {
            logger.LogError(ex, "Unable to process file - CorruptImageException {FileName}", file.FullName);
            return new MediaEntry(file, hash) { Notes = $"Corrupt image provided {file.FullName}" };
        }
    }

    private async Task<MediaEntry> ProcessMedia(
        MagickImage image,
        FileInfo file,
        string hash,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity();
        var histogram = mediaColourProcessor.GetBaseImageColour(image);
        var (capturedUtc, capturedOffset) = await mediaDateTimeProcessor.MediaCaptureDate(file, cancellationToken);
        var (longitudeDirection, latitudeDirection, latitudeResult, longitudeResult, cachedLocation) =
            await mediaLocationProcessor.ProcessLocation(file, cancellationToken);
        var entry = new MediaEntry(file, hash)
        {
            Enabled = true,
            BaseColourB = histogram.r,
            BaseColourG = histogram.g,
            BaseColourR = histogram.b,
            ImageRatio = image.Width / (double)image.Height,
            ImagePortrait = image.Height >= image.Width,
            CapturedUtc = capturedUtc,
            CapturedOffset = capturedOffset,
            LongitudeDirection = longitudeDirection,
            LatitudeDirection = latitudeDirection,
            Latitude = latitudeResult,
            Longitude = longitudeResult,
            LocationLabel = cachedLocation,
            Notes = JsonSerializer.Serialize(
                image.AttributeNames.Select(name => (Key: name, Value: image.GetAttribute(name))).ToDictionary()
            )
        };

        var directory = mediaPaths.GetTransformDirectory(entry.OriginalHash);
        var originalInfo = new FileInfo(Path.Combine(directory.FullName, "original" + file.Extension));
        if (originalInfo.Exists) return entry;

        logger.LogDebug("Writing Original File {OriginalFile} {CachedFile}", file.FullName, originalInfo.FullName);
        if (!originalInfo.Exists) file.CopyTo(originalInfo.FullName);

        return entry;
    }
}
