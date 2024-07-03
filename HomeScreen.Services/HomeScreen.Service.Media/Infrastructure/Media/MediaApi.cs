using HomeScreen.Database.MediaDb.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaApi(
    ILogger<MediaApi> logger,
    IMediaPaths mediaPaths,
    IMediaHasher mediaHasher,
    IMediaProcessor mediaProcessor,
    MediaDbContext context
) : IMediaApi
{
    public async Task<IList<MediaEntry>> GetRandomMedia(uint count, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetRandomMedia {Count}", count);
        var files = mediaPaths.GetRawFiles();
        var fileHashes = await Task.WhenAll(
            Random.Shared.GetItems(files.ToArray(), (int)count)
                .Select(
                    async file =>
                    {
                        var hash = await mediaHasher.HashMedia(file, cancellationToken);
                        var entry = await context.MediaEntries.Where(entry => entry.OriginalHash == hash)
                            .FirstOrDefaultAsync(cancellationToken);
                        if (entry == null)
                        {
                            entry = await mediaProcessor.ProcessMediaEntry(file, hash, cancellationToken);
                            await context.MediaEntries.AddAsync(entry, cancellationToken);
                        }

                        return entry;
                    }
                )
        );
        await context.SaveChangesAsync(cancellationToken);

        return fileHashes.Select(TransformMediaEntry).ToList();
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
