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
    var disabled = await context.MediaEntries.Where(entry => !entry.Enabled).ToListAsync(cancellationToken);
    var files = mediaPaths.GetRawFiles()
        .Where(f => !disabled.Exists(d => d.OriginalFile.Contains(f.FullName)))
        .ToArray();
    if (files.Length == 0)
    {
        logger.LogInformation("GetRandomMedia End {Count}", count);
        yield break;
    }

    foreach (var file in Random.Shared.GetItems(files, (int)count))
    {
        using var enumeratorActivity = ActivitySource.StartActivity();
        enumeratorActivity?.AddBaggage("FileName", file.FullName);
        cancellationToken.ThrowIfCancellationRequested();
        var hash = await mediaHasher.HashMedia(file, cancellationToken);
        enumeratorActivity?.AddBaggage("Hash", hash);
        var entry = await context.MediaEntries.Where(entry => entry.OriginalHash == hash)
            .FirstOrDefaultAsync(cancellationToken);
        if (entry is null)
        {
            entry = await mediaProcessor.ProcessMediaEntry(file, hash, cancellationToken);
            await context.MediaEntries.AddAsync(entry, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("GetRandomMedia Progress {FileName}", file.Name);
        yield return TransformMediaEntry(entry);
    }

    logger.LogInformation("GetRandomMedia End {Count}", count);
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
    logger.LogInformation("Attempting to download media for {MediaId}", mediaId);
    var mediaEntry = await context.MediaEntries.Where(x => x.MediaId == mediaId)
        .FirstOrDefaultAsync(cancellationToken);
    if (mediaEntry == null)
    {
        logger.LogInformation("No media found {MediaId} while try to transform media", mediaId);
        return TransformState.NotFound;
    }

    logger.LogInformation("Attempting to get transformed media for {MediaId}", mediaId);
    return TransformState.Transformed;
}

public async Task<(FileInfo, DateTimeOffset)?> GetTransformedMedia(
    Guid mediaId,
    MediaTransformOptions options,
    CancellationToken cancellationToken
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogInformation("Attempting to download media for {MediaId}", mediaId);
    var mediaEntry = await context.MediaEntries.Where(x => x.MediaId == mediaId)
        .FirstOrDefaultAsync(cancellationToken);
    if (mediaEntry == null)
    {
        logger.LogInformation("No media found {MediaId} while try to transform media", mediaId);
        return null;
    }

    logger.LogInformation("Attempting to get transformed media for {MediaId}", mediaId);
    return (await mediaTransformer.GetTransformedMedia(mediaEntry, options, cancellationToken),
            mediaEntry.CapturedUtc.ToOffset(mediaEntry.CapturedOffset));
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
