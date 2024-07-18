using HomeScreen.Service.Media.Entities;
using ImageMagick;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaTransformer(ILogger<MediaTransformer> logger, IMediaPaths mediaPaths) : IMediaTransformer
{
    public async Task<TransformState> TransformedMedia(
        Database.MediaDb.Entities.MediaEntry mediaEntry,
        MediaTransformOptions options,
        CancellationToken cancellationToken
    )
    {
        var transformedInfo = mediaPaths.GetCachePath(options, mediaEntry.OriginalHash);
        if (transformedInfo.Exists)
        {
            logger.LogInformation(
                "{OriginalPath} has already been transformed at {TransformedPath}",
                mediaEntry.OriginalFile,
                transformedInfo.FullName
            );
            return TransformState.Transformed;
        }

        logger.LogInformation(
            "Image {OriginalPath} has no cached transform, attempting to transform it {TransformedPath}",
            mediaEntry.OriginalFile,
            transformedInfo.FullName
        );
        var info = new FileInfo(mediaEntry.OriginalFile);
        using var image = new MagickImage();
        await image.ReadAsync(info, cancellationToken);
        image.AutoOrient();

        if (options.Blur)
        {
            image.FilterType = FilterType.Gaussian;
            image.Blur(0, 5);
        }
        else
        {
            image.InterpolativeResize(options.Width, options.Height, PixelInterpolateMethod.Mesh);
        }

        image.Format = options.Format.TransformFormatToMagickFormat();

        logger.LogInformation(
            "Image has been transformed {OriginalPath} {TransformedPath}",
            mediaEntry.OriginalFile,
            transformedInfo.FullName
        );
        await image.WriteAsync(transformedInfo, cancellationToken);
        return TransformState.Transformed;
    }

    public FileInfo? GetTransformedMedia(
        Database.MediaDb.Entities.MediaEntry mediaEntry,
        MediaTransformOptions options
    )
    {
        var transformedInfo = mediaPaths.GetCachePath(options, mediaEntry.OriginalHash);
        if (!transformedInfo.Exists) return null;
        
        logger.LogInformation(
            "{OriginalPath} has already been transformed at {TransformedPath}",
            mediaEntry.OriginalFile,
            transformedInfo.FullName
        );
        return transformedInfo;

    }
}
