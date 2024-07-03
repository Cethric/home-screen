using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaHasher(ILogger<MediaHasher> logger, IDistributedCache distributedCache) : IMediaHasher
{
    public async Task<string> HashMedia(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Hashing file {FileName}", fileInfo.FullName);
        var hash = await distributedCache.GetStringAsync(fileInfo.FullName, cancellationToken);
        if (!string.IsNullOrEmpty(hash))
        {
            logger.LogInformation("Found cached hash for file {FileName} -  {Hash}", fileInfo.FullName, hash);
            return hash;
        }

        try
        {
            using var hasher = SHA256.Create();
            await using var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read);
            fileStream.Position = 0;
            var fileHash = await hasher.ComputeHashAsync(fileStream, cancellationToken);
            hash = BitConverter.ToString(fileHash).Replace("-", string.Empty);
            await distributedCache.SetStringAsync(fileInfo.FullName, hash, cancellationToken);
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
        await distributedCache.SetStringAsync(fileInfo.FullName, hash, cancellationToken);
        return hash;
    }
}
