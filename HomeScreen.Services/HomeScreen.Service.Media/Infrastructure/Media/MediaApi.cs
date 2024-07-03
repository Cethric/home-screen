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
    public async Task<IList<MediaEntry>> GetRandomMedia(uint count, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetRandomMedia {Count}", count);
        var files = mediaPaths.GetRawFiles();
        var entries = new List<Database.MediaDb.Entities.MediaEntry>();
        foreach (var file in Random.Shared.GetItems(files.ToArray(), (int)count))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var hash = await mediaHasher.HashMedia(file, cancellationToken);
            var entry = await context.MediaEntries.Where(entry => entry.OriginalHash == hash)
                .FirstOrDefaultAsync(cancellationToken);
            if (entry is null)
            {
                entry = await mediaProcessor.ProcessMediaEntry(file, hash, cancellationToken);
                await context.MediaEntries.AddAsync(entry, cancellationToken);
            }

            entries.Add(entry);
        }

        await context.SaveChangesAsync(cancellationToken);

        return entries.Select(TransformMediaEntry).ToList();
    }

    public async Task<MediaEntry> ToggleMedia(Guid mediaId, bool state, CancellationToken cancellationToken)
    {
        var mediaEntry = await context.MediaEntries.Where(x => Equals(x.MediaId, mediaId))
            .FirstOrDefaultAsync(cancellationToken);
        if (mediaEntry == null) throw new ArgumentException("Invalid Media ID provided", nameof(mediaId));

        mediaEntry.Enabled = state;
        context.Update(mediaEntry);
        await context.SaveChangesAsync(cancellationToken);

        return TransformMediaEntry(mediaEntry);
    }

    public async Task<FileInfo?> GetTransformedMedia(
        Guid mediaId,
        MediaTransformOptions options,
        CancellationToken cancellationToken
    )
    {
        var mediaEntry = await context.MediaEntries.Where(x => x.MediaId == mediaId)
            .FirstOrDefaultAsync(cancellationToken);
        if (mediaEntry == null)
        {
            logger.LogInformation("No media found {MediaId} while try to transform media", mediaId);
            return null;
        }

        logger.LogInformation("Attempting to get transformed media for {MediaId}", mediaId);
        return await mediaTransformer.GetTransformedMedia(mediaEntry, options, cancellationToken);
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
            UtcDatetime = entry.CapturedUtc.ToUnixTimeMilliseconds()
        };
}
