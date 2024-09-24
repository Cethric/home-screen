using System.Diagnostics;
using HomeScreen.Service.Media.Configuration;
using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaPaths(ILogger<MediaPaths> logger, MediaDirectories mediaDirectories) : IMediaPaths
{
    private static readonly string[] AllowedImageExtensions =
    [
        ".jpeg",
        ".jpg",
        ".png",
        ".heic",
        ".gif",
        ".tif",
        ".tiff"
    ];

private static ActivitySource ActivitySource => new(nameof(MediaHasher));

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
        Path.Combine(
            mediaDirectories.MediaCacheDir,
            "cached",
            $"{first:03d}",
            $"{second:03d}",
            $"{third:03d}"
        )
    );
    directory.Create();
    logger.LogDebug(
        "Transform directory for {FileHash} is created at {Directory}",
        fileHash,
        directory.FullName
    );
    return directory;
}

public FileInfo GetCachePath(MediaTransformOptions mediaTransformOptions, string fileHash)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug(
        "Getting cache path for {FileHash} with transform options {Width} {Height} {Blur} {Format}",
        fileHash,
        mediaTransformOptions.Width,
        mediaTransformOptions.Height,
        mediaTransformOptions.Blur,
        mediaTransformOptions.Format
    );
    var directory = GetTransformDirectory(fileHash);
    var fileInfo = new FileInfo(
        Path.Combine(
            directory.FullName,
            $"transformed-{mediaTransformOptions.Width}-{mediaTransformOptions.Height}-{mediaTransformOptions.Blur}.{mediaTransformOptions.Format}"
        )
    );
    logger.LogDebug(
        "Retrieved cache path for {FileHash} with transform options {Width} {Height} {Blur} {Format}. {FileName}",
        fileHash,
        mediaTransformOptions.Width,
        mediaTransformOptions.Height,
        mediaTransformOptions.Blur,
        mediaTransformOptions.Format,
        fileInfo
    );
    return fileInfo;
}

private async Task<IEnumerable<string>> GetFileNames(CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug("Getting file names from {SearchDirectory}", mediaDirectories.MediaSourceDir);
    var files = await Directory
        .EnumerateFiles(mediaDirectories.MediaSourceDir, "*.*", SearchOption.TopDirectoryOnly)
        .ToAsyncEnumerable()
        .WhereAwaitWithCancellation(
            (f, cancelation) => AllowedImageExtensions.ToAsyncEnumerable()
                .ContainsAsync(Path.GetExtension(f).ToLowerInvariant(), cancelation)
        )
        .ToListAsync(cancellationToken);
    logger.LogDebug("Found files names from {SearchDirectory}", mediaDirectories.MediaSourceDir);
    return files;
}

public async Task<IEnumerable<FileInfo>> GetRawFiles(CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug("Getting raw files from {SearchDirectory}", mediaDirectories.MediaSourceDir);
    var fileNames = await GetFileNames(cancellationToken);
    var files = fileNames.Select(f => new FileInfo(f));
    logger.LogDebug("Found raw files in {SearchDirectory}", mediaDirectories.MediaSourceDir);
    return files;
}

public async Task<ulong> TotalMedia(CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity();
    var fileNames = await GetFileNames(cancellationToken);
    var count = fileNames.Count();
    if (count <= 0)
    {
        return 0;
    }

    return (ulong)count;
}
}
