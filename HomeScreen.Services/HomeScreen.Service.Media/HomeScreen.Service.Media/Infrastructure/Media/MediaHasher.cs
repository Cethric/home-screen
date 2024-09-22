using System.Diagnostics;
using System.Security.Cryptography;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaHasher(ILogger<MediaHasher> logger, IGenericCache genericCache) : IMediaHasher
{
    private static ActivitySource ActivitySource => new(nameof(MediaHasher));

    public async Task<string> HashMedia(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogInformation("Hashing file {FileName}", fileInfo.FullName);
        activity?.AddBaggage("FullName", fileInfo.FullName);
        var hash = await genericCache.ReadCache(fileInfo.FullName, cancellationToken);
        if (!string.IsNullOrEmpty(hash))
        {
            activity?.AddBaggage("Hash", hash);
            logger.LogInformation("Found cached hash for file {FileName} -  {Hash}", fileInfo.FullName, hash);
            return hash;
        }

        try
        {
            hash = await HashMediaEntry(fileInfo, cancellationToken);
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

        logger.LogInformation("Hashing file {FileName} failed. Using random GUID", fileInfo.FullName);
        hash = Guid.NewGuid().ToString("N");
        activity?.AddBaggage("Hash", hash);
        await genericCache.WriteCache(fileInfo.FullName, hash, cancellationToken);
        return hash;
    }

    private async Task<string> HashMediaEntry(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddBaggage("FullName", fileInfo.FullName);
        using var hasher = SHA256.Create();
        await using var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read);
        fileStream.Position = 0;
        var fileHash = await hasher.ComputeHashAsync(fileStream, cancellationToken);
        var hash = BitConverter.ToString(fileHash).Replace("-", string.Empty);
        activity?.AddBaggage("Hash", hash);
        await genericCache.WriteCache(fileInfo.FullName, hash, cancellationToken);
        return hash;
    }
}
