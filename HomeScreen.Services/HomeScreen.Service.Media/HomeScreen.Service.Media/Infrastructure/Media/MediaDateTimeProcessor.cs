using System.Diagnostics;
using System.Globalization;
using MetadataExtractor;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaDateTimeProcessor(ILogger<MediaDateTimeProcessor> logger, IGenericCache genericCache)
    : IMediaDateTimeProcessor
{
    private class CacheEntry
{
    public DateTimeOffset DateTime { get; init; }
    public TimeSpan Offset { get; init; }
}

private static ActivitySource ActivitySource => new(nameof(MediaDateTimeProcessor));

public async Task<(DateTimeOffset, TimeSpan)> MediaCaptureDate(
    FileInfo file,
    string hash,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    var cache = await genericCache.ReadCache<CacheEntry>(CacheKey(hash), cancellationToken);
    if (cache is not null)
    {
        return (cache.DateTime, cache.Offset);
    }

    logger.LogInformation("Determining media date for {FileName}", file.FullName);
    try
    {
        var metadata = ImageMetadataReader.ReadMetadata(file.FullName);
        var exif = await metadata.ToAsyncEnumerable()
            .Where(data => string.Equals(data.Name, "Exif SubIfd"))
            .FirstOrDefaultAsync(cancellationToken);
        var dateTime = exif?.Tags.FirstOrDefault(tag => tag.Type == 36867)?.Description;
        var offset = exif?.Tags.FirstOrDefault(tag => tag.Type == 36880)?.Description;
        if (!DateTimeOffset.TryParse(dateTime, CultureInfo.InvariantCulture, out var dateTimeOffset) ||
            !TimeSpan.TryParse(offset, CultureInfo.InvariantCulture, out var offsetSpan))
        {
            return await ProcessFileTime(file, hash, cancellationToken);
        }

        await genericCache.WriteCache(
            CacheKey(hash),
            new CacheEntry { DateTime = dateTimeOffset, Offset = offsetSpan },
            cancellationToken
        );
        logger.LogInformation("Using media capture date {FileName}", file.FullName);
        return (dateTimeOffset, offsetSpan);
    }
    catch (IOException ex)
    {
        logger.LogError(ex, "Unable to process file - IOException {FileName}", file.FullName);
    }
    catch (UnauthorizedAccessException ex)
    {
        logger.LogError(ex, "Unable to process file - AccessException {FileName}", file.FullName);
    }

    logger.LogInformation("Using media file date {FileName}", file.FullName);
    return await ProcessFileTime(file, hash, cancellationToken);
}

private static string CacheKey(string hash) => $"FileDate:${hash}";

private async Task<(DateTimeOffset, TimeSpan)> ProcessFileTime(
    FileInfo file,
    string hash,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    var cache = GetFileTime(file);
    await genericCache.WriteCache(CacheKey(hash), cache, cancellationToken);
    return (cache.DateTime, cache.Offset);
}

private static CacheEntry GetFileTime(FileInfo info)
{
    using var activity = ActivitySource.StartActivity();
    if (info.LastWriteTimeUtc < info.CreationTimeUtc)
    {
        return new CacheEntry
        {
            DateTime = info.LastWriteTimeUtc,
            Offset = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time")
                       .GetUtcOffset(info.LastWriteTime)
        };
    }

    return new CacheEntry
    {
        DateTime = info.CreationTimeUtc,
        Offset = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time")
                   .GetUtcOffset(info.CreationTime)
    };
}
}
