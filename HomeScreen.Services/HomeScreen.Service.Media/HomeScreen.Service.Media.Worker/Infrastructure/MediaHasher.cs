using System.Diagnostics;
using System.Security.Cryptography;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaHasher(ILogger<MediaHasher> logger) : IMediaHasher
{
    private static ActivitySource ActivitySource => new(nameof(MediaHasher));

    public async Task<string> HashMedia(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogDebug("Hashing file {FileName}", fileInfo.FullName);
        activity?.AddBaggage("FullName", fileInfo.FullName);

        try
        {
            var hash = await HashMediaEntry(fileInfo, cancellationToken);
            activity?.AddBaggage("Hash", hash);
            return hash;
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, "Unable to hash file {FileName}", fileInfo.FullName);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unable to hash file {FileName}", fileInfo.FullName);
        }

        logger.LogDebug("Hashing file {FileName} failed. Using random GUID", fileInfo.FullName);
        var guid = Guid.NewGuid().ToString("N");
        activity?.AddBaggage("Hash", guid);
        return guid;
    }

    private static async Task<string> HashMediaEntry(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddBaggage("FullName", fileInfo.FullName);
        using var hasher = SHA256.Create();
        await using var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read);
        fileStream.Position = 0;
        var fileHash = await hasher.ComputeHashAsync(fileStream, cancellationToken);
        var hash = BitConverter.ToString(fileHash).Replace("-", string.Empty);
        activity?.AddBaggage("Hash", hash);
        return hash;
    }
}
