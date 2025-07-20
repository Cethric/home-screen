using System.Diagnostics;
using HomeScreen.Service.Media.Common.Configuration;
using Microsoft.Extensions.Logging;

namespace HomeScreen.Service.Media.Common.Infrastructure.Media;

public class MediaPaths(ILogger<MediaPaths> logger, MediaDirectories mediaDirectories) : IMediaPaths
{
    private static readonly string[] AllowedImageExtensions =
    [
        "*.jpeg", "*.jpg", "*.png", "*.heic", "*.gif", "*.tif", "*.tiff"
    ];

    private static ActivitySource ActivitySource => new(nameof(MediaPaths));

    public DirectoryInfo GetTransformDirectory(string fileHash)
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogDebug("Getting transform directory for {FileHash}", fileHash);
        var hash = fileHash.GetHashCode();
        const int mask = 255;
        var first = hash & mask;
        var second = (hash >> 8) & mask;
        var third = (hash >> 16) & mask;
        var directory = new DirectoryInfo(
            Path.Combine(mediaDirectories.MediaCacheDir, "cached", $"{first:03d}", $"{second:03d}", $"{third:03d}")
        );
        directory.Create();
        logger.LogDebug("Transform directory for {FileHash} is created at {Directory}", fileHash, directory.FullName);
        return directory;
    }

    public IAsyncEnumerable<string> GetRawFiles()
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogTrace("Getting raw files from {SearchDirectory}", mediaDirectories.MediaSourceDir);
        return GetFileNames();
    }

    private IAsyncEnumerable<string> GetFileNames()
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogTrace("Getting file names from {SearchDirectory}", mediaDirectories.MediaSourceDir);
        var files = AllowedImageExtensions
            .AsParallel()
            .SelectMany(ext => Directory.EnumerateFiles(
                    mediaDirectories.MediaSourceDir,
                    ext,
                    SearchOption.AllDirectories
                )
            )
            .ToAsyncEnumerable();
        logger.LogTrace("Found files names from {SearchDirectory}", mediaDirectories.MediaSourceDir);
        return files;
    }
}
