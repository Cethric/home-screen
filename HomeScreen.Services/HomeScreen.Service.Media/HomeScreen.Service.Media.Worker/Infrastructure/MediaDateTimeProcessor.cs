using System.Diagnostics;
using System.Globalization;
using MetadataExtractor;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaDateTimeProcessor(ILogger<MediaDateTimeProcessor> logger, IMediaMetadataReader mediaMetadataReader)
    : IMediaDateTimeProcessor
{
    private static ActivitySource ActivitySource => new(nameof(MediaDateTimeProcessor));

    public async Task<(DateTimeOffset, TimeSpan)> MediaCaptureDate(
        FileInfo file,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();

        var exif = await mediaMetadataReader.LoadExif(file, cancellationToken);
        
        logger.LogDebug("Determining media date for {FileName}", file.FullName);
        var dateTime = exif?.Tags.FirstOrDefault(tag => tag.Type == 36867)?.Description;
        var offset = exif?.Tags.FirstOrDefault(tag => tag.Type == 36880)?.Description;
        if (!DateTimeOffset.TryParse(dateTime, CultureInfo.InvariantCulture, out var dateTimeOffset) ||
            !TimeSpan.TryParse(offset, CultureInfo.InvariantCulture, out var offsetSpan))
        {
            logger.LogDebug("Unable to determine capture date from metadata {FileName}", file.FullName);
            return GetFileTime(file);
        }

        logger.LogDebug("Using media capture date {FileName}", file.FullName);
        return (dateTimeOffset, offsetSpan);
    }

    private static (DateTimeOffset, TimeSpan) GetFileTime(FileInfo info)
    {
        using var activity = ActivitySource.StartActivity();
        if (info.LastWriteTimeUtc < info.CreationTimeUtc)
            return (info.LastWriteTimeUtc,
                TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time").GetUtcOffset(info.LastWriteTime));

        return (info.CreationTimeUtc,
            TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time").GetUtcOffset(info.CreationTime));
    }
}
