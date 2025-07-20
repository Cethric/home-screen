using System.Diagnostics;
using HomeScreen.Database.MediaDb.Contexts;
using HomeScreen.Service.Media.Common.Infrastructure.Media;
using Microsoft.EntityFrameworkCore;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaApi(
    ILogger<Worker> logger,
    IMediaPaths mediaPaths,
    IMediaHasher mediaHasher,
    IMediaProcessor mediaProcessor,
    MediaDbContext context
) : IMediaApi
{
    private static ActivitySource ActivitySource => new(nameof(MediaPaths));

    public async Task ProcessMedia(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity();
        cancellationToken.ThrowIfCancellationRequested();
        logger.LogTrace("Process Media Entries");
        await foreach (var entry in mediaPaths.GetRawFiles().WithCancellation(cancellationToken))
        {
            logger.LogInformation("Process Media Entry {MediaEntry}", entry);
            var file = new FileInfo(entry);
            var hash = await mediaHasher.HashMedia(file, cancellationToken);
            await ProcessMediaFile(file, hash, cancellationToken);
            logger.LogInformation("Processed Media Entry {MediaEntry}", entry);
        }

        logger.LogTrace("Processed Media Entries");
    }

    private async Task ProcessMediaFile(FileInfo file, string hash, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity();
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            logger.LogInformation("Adding file {FileName} ({FileHash}) to the cache", file, hash);

            var containsHash = await context.MediaEntries.AnyAsync(
                entry => entry.OriginalHash.Equals(hash),
                cancellationToken
            );
            var containsFile = await context.MediaEntries.AnyAsync(
                entry => entry.OriginalFile.Equals(file.FullName),
                cancellationToken
            );

            if (containsHash)
            {
                logger.LogInformation("File {FileName} ({FileHash}) already in the database", file, hash);
                return;
            }

            if (containsFile && !containsHash)
            {
                logger.LogWarning(
                    "File {FileName} ({FileHash}) already in the database, but with different hash. Skipping entry for now",
                    file,
                    hash
                );
                return;
            }

            logger.LogInformation("File {FileName} ({FileHash}) needs to be added to the db cache", file, hash);

            var entry = await mediaProcessor.ProcessMediaEntry(file, hash, cancellationToken);

            await context.MediaEntries.AddAsync(entry, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("File {FileName} ({FileHash}) has been added to the db cache", file, hash);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process media file {FileName}", file);
        }
    }
}
