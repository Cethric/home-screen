using System.Diagnostics;
using System.Runtime.CompilerServices;
using HomeScreen.Database.MediaDb.Contexts;
using HomeScreen.Service.Media.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaApi(
    ILogger<MediaApi> logger,
    IMediaPaths mediaPaths,
    IMediaHasher mediaHasher,
    IMediaProcessor mediaProcessor,
    IMediaTransformer mediaTransformer,
    IMediaDateTimeProcessor mediaDateTimeProcessor,
    MediaDbContext context
) : IMediaApi
{
    private static ActivitySource ActivitySource => new(nameof(MediaApi));

public async IAsyncEnumerable<MediaEntry> GetRandomMedia(
    uint count,
    [EnumeratorCancellation] CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("Count", count.ToString());
    logger.LogDebug("GetRandomMedia Start {Count}", count);
    var disabled = await context.MediaEntries.Where(entry => !entry.Enabled).ToListAsync(cancellationToken);
    var rawFiles = await mediaPaths.GetRawFiles(cancellationToken);
    var files = await rawFiles.ToAsyncEnumerable()
        .WhereAwaitWithCancellation(
            async (file, cancellation) => !(await disabled.ToAsyncEnumerable()
                .AnyAsync((entry) => entry.OriginalFile.Contains(file.FullName), cancellation))
        )
        .SelectAwaitWithCancellation(
            async (file, cancellation) =>
            {
                var hash = await mediaHasher.HashMedia(file, cancellation);
                return (file, hash);
            }
        )
        .ToArrayAsync(cancellationToken);
    if (files.Length == 0)
    {
        logger.LogDebug("GetRandomMedia End {Count}", count);
        yield break;
    }

    await foreach (var entry in EnumerateMedia(Random.Shared.GetItems(files, (int)count), cancellationToken))
    {
        yield return entry;
    }

    logger.LogDebug("GetRandomMedia End {Count}", count);
}

public async Task<MediaEntry> ToggleMedia(Guid mediaId, bool state, CancellationToken cancellationToken)
{
    using var activity = ActivitySource.StartActivity();
    var mediaEntry = await context.MediaEntries.Where(x => Equals(x.MediaId, mediaId))
        .FirstOrDefaultAsync(cancellationToken);
    if (mediaEntry == null)
    {
        throw new ArgumentException("Invalid Media ID provided", nameof(mediaId));
    }

    mediaEntry.Enabled = state;
    context.Update(mediaEntry);
    await context.SaveChangesAsync(cancellationToken);

    return TransformMediaEntry(mediaEntry);
}

public async Task<TransformState> TransformMedia(
    Guid mediaId,
    MediaTransformOptions options,
    CancellationToken cancellationToken
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug("Attempting to transform media for {MediaId}", mediaId);
    var mediaEntry = await context.MediaEntries.Where(x => x.MediaId == mediaId)
        .FirstOrDefaultAsync(cancellationToken);
    if (mediaEntry == null)
    {
        logger.LogDebug("No media found {MediaId} while try to transform media", mediaId);
        return TransformState.NotFound;
    }

    logger.LogDebug("Transformed media for {MediaId}", mediaId);
    return TransformState.Transformed;
}

public async Task<(FileInfo, DateTimeOffset)?> GetTransformedMedia(
    Guid mediaId,
    MediaTransformOptions options,
    CancellationToken cancellationToken
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug("Attempting to download media for {MediaId}", mediaId);
    var mediaEntry = await context.MediaEntries.Where(x => x.MediaId == mediaId)
        .FirstOrDefaultAsync(cancellationToken);
    if (mediaEntry == null)
    {
        logger.LogDebug("No media found {MediaId} while try to transform media", mediaId);
        return null;
    }

    logger.LogDebug("Attempting to get transformed media for {MediaId}", mediaId);
    return (await mediaTransformer.GetTransformedMedia(mediaEntry, options, cancellationToken),
            mediaEntry.CapturedUtc.ToOffset(mediaEntry.CapturedOffset));
}

public async IAsyncEnumerable<(MediaEntry, ulong)> GetPaginatedMedia(
    int requestOffset,
    int requestLength,
    [EnumeratorCancellation] CancellationToken cancellationToken
)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("Offset", requestOffset.ToString());
    activity?.AddBaggage("Length", requestLength.ToString());
    logger.LogDebug("GetPaginatedMedia Start {Offset}, {Length}", requestOffset, requestLength);
    cancellationToken.ThrowIfCancellationRequested();
    var rawFiles = await (await mediaPaths.GetRawFiles(cancellationToken)).ToAsyncEnumerable()
        .ToListAsync(cancellationToken);
    var total = rawFiles.Count;
    if (total == 0)
    {
        logger.LogDebug("GetPaginatedMedia End {Offset}, {Length}", requestOffset, requestLength);
        yield break;
    }

    var files = await rawFiles.ToAsyncEnumerable()
        .SelectAwaitWithCancellation(
            async (file, cancellation) =>
            {
                var hash = await mediaHasher.HashMedia(file, cancellation);
                var (dateTimeOffset, offset) =
                    await mediaDateTimeProcessor.MediaCaptureDate(file, hash, cancellation);
                var unix = dateTimeOffset.ToOffset(offset).ToUnixTimeMilliseconds();
                return (file, hash, unix);
            }
        )
        .OrderBy(item => item.unix)
        .Skip(requestOffset)
        .Take(requestLength)
        .Select(item => (item.file, item.hash))
        .ToListAsync(cancellationToken);
    if (files.Count == 0)
    {
        logger.LogDebug("GetPaginatedMedia End {Offset}, {Length}", requestOffset, requestLength);
        yield break;
    }

    await foreach (var entry in EnumerateMedia(files, cancellationToken))
    {
        yield return (entry, (ulong)rawFiles.Count);
    }

    logger.LogDebug("GetPaginatedMedia End {Offset}, {Length}", requestOffset, requestLength);
}

private async IAsyncEnumerable<MediaEntry> EnumerateMedia(
    IList<(FileInfo, string)> mediaEntries,
    [EnumeratorCancellation] CancellationToken cancellationToken
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug("EnumerateMedia Start");
    cancellationToken.ThrowIfCancellationRequested();

    if (mediaEntries.Count == 0)
    {
        logger.LogDebug("EnumerateMedia End");
        yield break;
    }

    foreach (var (file, hash) in mediaEntries)
    {
        using var enumeratorActivity = ActivitySource.StartActivity();
        enumeratorActivity?.AddBaggage("FileName", file.FullName);
        enumeratorActivity?.AddBaggage("Hash", hash);
        var entry = await context.MediaEntries.Where(entry => entry.OriginalHash == hash)
            .FirstOrDefaultAsync(cancellationToken);

        if (entry is null)
        {
            entry = await mediaProcessor.ProcessMediaEntry(file, hash, cancellationToken);
            await context.MediaEntries.AddAsync(entry, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogTrace("EnumerateMedia Progress {FileName}", file.Name);
        yield return TransformMediaEntry(entry);
    }

    await context.SaveChangesAsync(cancellationToken);

    logger.LogDebug("EnumerateMedia End");
}

private static MediaEntry TransformMediaEntry(Database.MediaDb.Entities.MediaEntry entry) =>
    new()
    {
        Id = entry.MediaId.ToString("D"),
        Enabled = entry.Enabled,
        Latitude = entry.Latitude,
        Longitude = entry.Longitude,
        Location = entry.LocationLabel,
        Notes = entry.Notes,
        UtcDatetime = entry.CapturedUtc.ToUnixTimeMilliseconds(),
        AspectRatioWidth = entry.ImageRatioWidth,
        AspectRatioHeight = entry.ImageRatioHeight,
        BaseB = entry.BaseColourB,
        BaseG = entry.BaseColourG,
        BaseR = entry.BaseColourR
    };
}
