using HomeScreen.Service.Media.Entities;
using ImageMagick;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaTransformer(ILogger<MediaTransformer> logger, IMediaPaths mediaPaths) : IMediaTransformer
{
    public async Task<FileInfo> GetTransformedMedia(
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
        return transformedInfo;
    }

    logger.LogInformation(
        "Image {OriginalPath} has no cached transform, attempting to transform it {TransformedPath}",
        mediaEntry.OriginalFile,
        transformedInfo.FullName
    );
    var stream = await File.ReadAllBytesAsync(mediaEntry.OriginalFile, cancellationToken);
    using var image = new MagickImage(stream);
    image.AutoOrient();

    if (options.Blur)
    {
        logger.LogInformation(
            "Blurring Image {TransformPath} to max size {Width}x{Height}",
            transformedInfo.FullName,
            options.Width,
            options.Height
        );
        image.FilterType = options is { Width: < 250 } or { Height: < 250 }
            ? FilterType.Point
            : FilterType.Gaussian;
        image.Depth = 4;
        image.Thumbnail(options.Width / 3, options.Height / 3);
        image.MedianFilter(4);
        image.Blur(0, 5);
        image.Resize(options.Width, options.Height);
        logger.LogInformation(
            "Blurred Image {TransformPath} to size {Width}x{Height} - requested {RequestedWidth}x{RequestedHeight}",
            transformedInfo.FullName,
            image.Width,
            image.Height,
            options.Width,
            options.Height
        );
    }
    else
    {
        image.FilterType = options is { Width: < 250 } or { Height: < 250 }
            ? FilterType.Point
            : FilterType.LanczosRadius;
        image.Format = options.Format.TransformFormatToMagickFormat();
        image.SetBitDepth(12, Channels.RGB);
        image.SetProfile(ColorProfile.SRGB, ColorTransformMode.HighRes);

        logger.LogInformation(
            "Resizing Image {TransformPath} to max size {Width}x{Height}",
            transformedInfo.FullName,
            options.Width,
            options.Height
        );
        image.InterpolativeResize(
            options.Width,
            options.Height,
            options is { Width: < 250 } or { Height: < 250 }
                ? PixelInterpolateMethod.Nearest
                : PixelInterpolateMethod.Mesh
        );
        logger.LogInformation(
            "Resized Image {TransformPath} to size {Width}x{Height} - requested {RequestedWidth}x{RequestedHeight}",
            transformedInfo.FullName,
            image.Width,
            image.Height,
            options.Width,
            options.Height
        );
    }

    logger.LogInformation(
        "Image has been transformed {OriginalPath} {TransformedPath}",
        mediaEntry.OriginalFile,
        transformedInfo.FullName
    );
    await image.WriteAsync(transformedInfo, cancellationToken);
    return transformedInfo;
}
}
