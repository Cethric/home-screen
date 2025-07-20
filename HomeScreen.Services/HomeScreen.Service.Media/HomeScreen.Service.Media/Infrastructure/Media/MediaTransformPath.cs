using System.Diagnostics;
using HomeScreen.Service.Media.Common.Infrastructure.Media;
using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaTransformPath(ILogger<MediaTransformPath> logger, IMediaPaths mediaPaths) : IMediaTransformPath
{
    private static ActivitySource ActivitySource => new(nameof(MediaTransformPath));

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
        var directory = mediaPaths.GetTransformDirectory(fileHash);
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
}
