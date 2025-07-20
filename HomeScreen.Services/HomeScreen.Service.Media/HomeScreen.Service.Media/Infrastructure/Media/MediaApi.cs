using System.Diagnostics;
using System.Runtime.CompilerServices;
using HomeScreen.Database.MediaDb.Contexts;
using HomeScreen.Service.Media.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaApi(ILogger<MediaApi> logger, IMediaTransformer mediaTransformer, MediaDbContext context) : IMediaApi
{
    private static ActivitySource ActivitySource => new(nameof(MediaApi));

public async IAsyncEnumerable<MediaEntry> GetRandomMedia(
    uint count,
    [EnumeratorCancellation] CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("Count", count.ToString());
    logger.LogInformation("GetRandomMedia Start {Count}", count);
    var enabled = await context.MediaEntries.Where(entry => entry.Enabled).ToArrayAsync(cancellationToken);
    logger.LogInformation(
        "Found {FileCount} total images, randomly selecting {Count} images",
        enabled.Length,
        count
    );

    if (enabled.Length == 0)
    {
        logger.LogDebug("GetRandomMedia Exit {Count}", count);
        yield break;
    }

    var entries = Random
        .Shared.GetItems(enabled, (int)Math.Min(count, enabled.LongLength))
        .Select(TransformMediaEntry);
    foreach (var entry in entries)
    {
        yield return entry;
    }

    logger.LogDebug("GetRandomMedia End {Count}", count);
}

public async Task<MediaEntry> ToggleMedia(Guid mediaId, bool state, CancellationToken cancellationToken)
{
    using var activity = ActivitySource.StartActivity();
    var mediaEntry = await context
        .MediaEntries.Where(x => Equals(x.MediaId, mediaId))
        .FirstOrDefaultAsync(cancellationToken);
    if (mediaEntry == null) throw new ArgumentException("Invalid Media ID provided", nameof(mediaId));

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
    var mediaEntry =
        await context.MediaEntries.Where(x => x.MediaId == mediaId).FirstOrDefaultAsync(cancellationToken);
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
    var mediaEntry =
        await context.MediaEntries.Where(x => x.MediaId == mediaId).FirstOrDefaultAsync(cancellationToken);
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

    var total = await context.MediaEntries.LongCountAsync(cancellationToken);

    var entries = context
        .MediaEntries.OrderBy(e => e.CapturedUtc)
        .Skip(requestOffset)
        .Take(requestLength)
        .ToAsyncEnumerable()
        .Select(TransformMediaEntry)
        .AsAsyncEnumerable();

    await foreach (var entry in entries.WithCancellation(cancellationToken))
    {
        using var enumeratorActivity = ActivitySource.StartActivity();
        enumeratorActivity?.AddBaggage("Id", entry.Id);
        yield return (entry, (ulong)total);
    }

    logger.LogDebug("GetPaginatedMedia End {Offset}, {Length}", requestOffset, requestLength);
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
        AspectRatio = entry.ImageRatio,
        Portrait = entry.ImagePortrait,
        BaseB = entry.BaseColourB,
        BaseG = entry.BaseColourG,
        BaseR = entry.BaseColourR
    };
}