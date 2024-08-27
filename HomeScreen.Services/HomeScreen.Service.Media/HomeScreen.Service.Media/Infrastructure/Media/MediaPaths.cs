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

public DirectoryInfo GetTransformDirectory(string fileHash)
{
    logger.LogInformation("Getting transform directory for {FileHash}", fileHash);
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
    logger.LogInformation(
        "Transform directory for {FileHash} is created at {Directory}",
        fileHash,
        directory.FullName
    );
    return directory;
}

public FileInfo GetCachePath(MediaTransformOptions mediaTransformOptions, string fileHash)
{
    logger.LogInformation(
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
    logger.LogInformation(
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

public List<FileInfo> GetRawFiles()
{
    logger.LogInformation("Getting raw files from {SearchDirectory}", mediaDirectories.MediaSourceDir);
    var files = Directory.EnumerateFiles(mediaDirectories.MediaSourceDir, "*.*", SearchOption.TopDirectoryOnly)
                         .Where(f => AllowedImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                         .Select(f => new FileInfo(f))
                         .ToList();
    logger.LogInformation(
        "Found {FilesCount} raw files in {SearchDirectory}",
        files.Count,
        mediaDirectories.MediaSourceDir
    );
    return files;
}
}
