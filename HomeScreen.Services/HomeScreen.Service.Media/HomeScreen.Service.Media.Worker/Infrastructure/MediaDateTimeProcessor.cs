using System.Diagnostics;
using System.Globalization;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaDateTimeProcessor(ILogger<MediaDateTimeProcessor> logger, IMediaMetadataReader mediaMetadataReader)
    : IMediaDateTimeProcessor
{
    private static ActivitySource ActivitySource => new(nameof(MediaDateTimeProcessor));

    private static readonly TimeZoneInfo DefaultTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

    // EXIF Tags for date/time information
    private const int TagDateTimeOriginal = 36867; // 0x9003 - When the original image was generated
    private const int TagDateTimeDigitized = 36868; // 0x9004 - When the image was stored as digital data
    private const int TagDateTime = 306; // 0x0132 - File change date and time
    private const int TagOffsetTime = 36880; // 0x9010 - Time zone for DateTimeOriginal
    private const int TagOffsetTimeOriginal = 36881; // 0x9011 - Time zone for DateTimeOriginal
    private const int TagOffsetTimeDigitized = 36882; // 0x9012 - Time zone for DateTimeDigitized
    private const int TagSubsecTimeOriginal = 37521; // 0x9291 - Fractions of seconds for DateTimeOriginal

    public async Task<(DateTimeOffset, TimeSpan)> MediaCaptureDate(
        FileInfo file,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogDebug("Determining media date for {FileName}", file.FullName);

        var exif = await mediaMetadataReader.LoadExif(file, cancellationToken);
        if (exif == null || !TryGetExifDateTimeOffset(file, exif, out var dateTime, out var offset))
        {
            return GetFileTime(file);
        }

        logger.LogInformation("Using DateTime from EXIF for {FileName} - {DateTime} {Offset}", file.FullName, dateTime, offset);
        return (dateTime, offset);
    }

    private bool TryGetExifDateTimeOffset(
        FileInfo file,
        ExifSubIfdDirectory exif,
        out DateTimeOffset dateTime,
        out TimeSpan offsetSpan
    )
    {
        (dateTime, offsetSpan) = GetFileTime(file);
        var parsed = false;

        if (TryGetExifDateTime(exif, TagDateTime, TagOffsetTime, out var dateTimeOffset, out var offset))
        {
            if (dateTime.UtcTicks > dateTimeOffset.UtcTicks)
            {
                dateTime = dateTimeOffset;
                offsetSpan = offset;
            }

            parsed = true;
        }

        if (TryGetExifDateTime(
                exif,
                TagDateTimeDigitized,
                TagOffsetTimeDigitized,
                out var dateTimeOffsetDigitized,
                out var offsetDigitized
            ))
        {
            if (dateTime.UtcTicks > dateTimeOffsetDigitized.UtcTicks)
            {
                dateTime = dateTimeOffsetDigitized;
                offsetSpan = offsetDigitized;
            }

            parsed = true;
        }

        if (!TryGetExifDateTime(
                exif,
                TagDateTimeOriginal,
                TagOffsetTimeOriginal,
                out var dateTimeOffsetOriginal,
                out var offsetOriginal
            ) ||
            dateTime.UtcTicks <= dateTimeOffsetOriginal.UtcTicks)
        {
            return parsed;
        }

        dateTime = dateTimeOffsetOriginal;
        offsetSpan = offsetOriginal;

        return true;
    }

    private bool TryGetExifDateTime(
        ExifSubIfdDirectory exif,
        int dateTimeTag,
        int offsetTag,
        out DateTimeOffset dateTime,
        out TimeSpan offsetSpan
    )
    {
        dateTime = default;
        offsetSpan = TimeSpan.Zero;

        if (!exif.TryGetDateTime(dateTimeTag, out var dt))
        {
            return false;
        }

        // Try to get subsecond info if available (for more precise timestamps)
        var subSec = exif.GetString(TagSubsecTimeOriginal);
        if (dateTimeTag == TagDateTimeOriginal && string.IsNullOrEmpty(subSec))
        {
            if (int.TryParse(subSec, out var milliseconds))
            {
                dt = dt.AddMilliseconds(milliseconds);
            }
        }

        var offsetStr = exif.GetString(offsetTag);
        if (string.IsNullOrEmpty(offsetStr))
        {
            // No offset specified, use default timezone
            dateTime = new DateTimeOffset(dt, DefaultTimeZone.GetUtcOffset(dt));
            offsetSpan = DefaultTimeZone.GetUtcOffset(dt);
            return true;
        }

        // Parse the offset string (format typically "+01:00" or "-05:00")
        if (TimeSpan.TryParse(offsetStr, CultureInfo.InvariantCulture, out offsetSpan) ||
            TryParseExifTimeOffset(offsetStr, out offsetSpan))
        {
            dateTime = new DateTimeOffset(dt, offsetSpan);
            return true;
        }

        // Fallback to default timezone if offset parsing fails
        dateTime = new DateTimeOffset(dt, DefaultTimeZone.GetUtcOffset(dt));
        offsetSpan = DefaultTimeZone.GetUtcOffset(dt);
        return true;
    }

    private bool TryParseExifTimeOffset(string offsetStr, out TimeSpan offset)
    {
        offset = TimeSpan.Zero;

        if (string.IsNullOrEmpty(offsetStr))
            return false;

        // Handle "+01:00" or "-05:00" format
        if (offsetStr.Length != 6 || (offsetStr[0] != '+' && offsetStr[0] != '-'))
        {
            return false;
        }

        if (!int.TryParse(offsetStr.AsSpan(1, 2), out var hours) ||
            !int.TryParse(offsetStr.AsSpan(4, 2), out var minutes))
        {
            return false;
        }

        offset = new TimeSpan(hours, minutes, 0);
        if (offsetStr[0] == '-')
            offset = offset.Negate();
        return true;
    }

    private (DateTimeOffset, TimeSpan) GetFileTime(FileInfo info)
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogDebug("Unable to determine capture date from metadata {FileName}", info.FullName);

        // Choose the earliest date between last write and creation time
        // This is more likely to be closer to the actual capture time
        var fileTime = info.LastWriteTimeUtc < info.CreationTimeUtc ? info.LastWriteTimeUtc : info.CreationTimeUtc;

        // For local file times, we can get the timezone offset
        var localTime = fileTime.ToLocalTime();
        var offsetSpan = DefaultTimeZone.GetUtcOffset(localTime);

        return (fileTime, offsetSpan);
    }
}
