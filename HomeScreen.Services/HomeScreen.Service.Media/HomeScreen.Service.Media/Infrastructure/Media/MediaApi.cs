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
        logger.LogInformation("GetRandomMedia Start {Count}", count);
        var disabled = context.MediaEntries.Where(entry => !entry.Enabled).Select(entry => entry.OriginalFile);
        var files = await mediaPaths
            .GetRawFiles()
            .ToAsyncEnumerable()
            .ExceptBy(disabled.ToAsyncEnumerable(), f => f.FullName, cancellationToken: cancellationToken)
            .SelectAwaitWithCancellation(async (file, cancellation) =>
                {
                    var hash = await mediaHasher.HashMedia(file, cancellation);
                    return (file, hash);
                }
            )
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
        logger.LogInformation("Found {FileCount} total images, randomly selecting {Count} images", files.Length, count);

        if (files.Length == 0)
        {
            logger.LogDebug("GetRandomMedia Exit {Count}", count);
            yield break;
        }

        var randomEntries = EnumerateMedia(
                Random.Shared.GetItems(files, (int)Math.Min(count, files.Length)),
                cancellationToken
            )
            .ConfigureAwait(false);

        await foreach (var entry in randomEntries) yield return entry;

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
        var rawFiles = await mediaPaths
            .GetRawFiles()
            .ToAsyncEnumerable()
            .SelectAwaitWithCancellation(async (file, cancellation) =>
                {
                    var hash = await mediaHasher.HashMedia(file, cancellation);
                    var (dateTimeOffset, offset) =
                        await mediaDateTimeProcessor.MediaCaptureDate(file, hash, cancellation);
                    var unix = dateTimeOffset.ToOffset(offset).ToUnixTimeMilliseconds();
                    return (file, hash, unix);
                }
            )
            .OrderBy(item => item.unix)
            .Select(item => (item.file, item.hash))
            .ToListAsync(cancellationToken);
        var total = rawFiles.Count;
        if (total == 0)
        {
            logger.LogDebug("GetPaginatedMedia End {Offset}, {Length}", requestOffset, requestLength);
            yield break;
        }

        var files = rawFiles.Skip(requestOffset).Take(requestLength).ToList();
        if (files.Count == 0)
        {
            logger.LogDebug("GetPaginatedMedia End {Offset}, {Length}", requestOffset, requestLength);
            yield break;
        }

        await foreach (var entry in EnumerateMedia(files, cancellationToken))
            yield return (entry, (ulong)rawFiles.Count);

        logger.LogDebug("GetPaginatedMedia End {Offset}, {Length}", requestOffset, requestLength);
    }

    private async IAsyncEnumerable<MediaEntry> EnumerateMedia(
        IList<(FileInfo, string)> mediaEntries,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogTrace("EnumerateMedia Start");
        cancellationToken.ThrowIfCancellationRequested();

        if (mediaEntries.Count == 0)
        {
            logger.LogTrace("EnumerateMedia End");
            yield break;
        }

        var entries = await context
            .MediaEntries.Where(entry => mediaEntries.Select(media => media.Item2).Contains(entry.OriginalHash))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        foreach (var entry in entries)
        {
            using var enumeratorActivity = ActivitySource.StartActivity();
            enumeratorActivity?.AddBaggage("FileName", entry.OriginalFile);
            enumeratorActivity?.AddBaggage("Hash", entry.OriginalHash);

            logger.LogInformation("EnumerateMedia Existing Item {FileName}", entry.OriginalFile);
            yield return TransformMediaEntry(entry);
        }


        var excludedIDs = new HashSet<string>(entries.Select(p => p.OriginalHash)).ToAsyncEnumerable();
        var newMediaEntries = mediaEntries
            .ToAsyncEnumerable()
            .WhereAwaitWithCancellation(async (entry, cancellation) =>
                !await excludedIDs.ContainsAsync(entry.Item2, cancellation)
            )
            .ConfigureAwait(false);

        await foreach (var (file, hash) in newMediaEntries)
        {
            using var enumeratorActivity = ActivitySource.StartActivity();
            enumeratorActivity?.AddBaggage("FileName", file.FullName);
            enumeratorActivity?.AddBaggage("Hash", hash);

            var entry = await mediaProcessor.ProcessMediaEntry(file, hash, cancellationToken).ConfigureAwait(false);
            await context.MediaEntries.AddAsync(entry, cancellationToken).ConfigureAwait(false);

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("EnumerateMedia New Item {FileName}", file.Name);
            yield return TransformMediaEntry(entry);
        }

        logger.LogTrace("EnumerateMedia End");
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
