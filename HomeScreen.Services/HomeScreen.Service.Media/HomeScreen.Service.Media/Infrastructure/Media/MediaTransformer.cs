using System.Diagnostics;
using HomeScreen.Service.Media.Entities;
using ImageMagick;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaTransformer(ILogger<MediaTransformer> logger, IMediaPaths mediaPaths) : IMediaTransformer
{
    private static ActivitySource ActivitySource => new(nameof(MediaHasher));

public async Task<FileInfo> GetTransformedMedia(
    Database.MediaDb.Entities.MediaEntry mediaEntry,
    MediaTransformOptions options,
    CancellationToken cancellationToken
)
{
    using var activity = ActivitySource.StartActivity();
    var transformedInfo = mediaPaths.GetCachePath(options, mediaEntry.OriginalHash);
    activity?.AddBaggage("MediaId", mediaEntry.MediaId.ToString("D"));
    activity?.AddBaggage("OriginalFile", mediaEntry.OriginalFile);
    activity?.AddBaggage("TransformedName", transformedInfo.FullName);
    if (transformedInfo.Exists)
    {
        logger.LogTrace(
            "{OriginalPath} has already been transformed at {TransformedPath}",
            mediaEntry.OriginalFile,
            transformedInfo.FullName
        );
        activity?.AddEvent(new ActivityEvent("Already Transformed"));
        return transformedInfo;
    }

    logger.LogDebug(
        "Image {OriginalPath} has no cached transform, attempting to transform it {TransformedPath}",
        mediaEntry.OriginalFile,
        transformedInfo.FullName
    );
    var stream = await File.ReadAllBytesAsync(mediaEntry.OriginalFile, cancellationToken);
    using var image = new MagickImage(stream);
    image.AutoOrient();

    if (options.Blur)
    {
        BlurImage(image, transformedInfo, options);
    }
    else
    {
        TransformImage(image, transformedInfo, options);
    }

    await WriteImage(image, mediaEntry, transformedInfo, cancellationToken);
    return transformedInfo;
}

private void BlurImage(MagickImage image, FileInfo transformedInfo, MediaTransformOptions options)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogTrace(
        "Blurring Image {TransformPath} to max size {Width}x{Height}",
        transformedInfo.FullName,
        options.Width,
        options.Height
    );
    image.FilterType = options is { Width: < 150 } or { Height: < 150 } ? FilterType.Point : FilterType.Gaussian;
    image.Depth = 4;
    image.Alpha(AlphaOption.Remove);
    image.Thumbnail(uint.Max(50, options.Width / 3), uint.Max(50, options.Height / 3));
    image.MedianFilter(4);
    image.Blur(0, 5);
    image.Resize(options.Width, options.Height);
    logger.LogTrace(
        "Blurred Image {TransformPath} to size {Width}x{Height} - requested {RequestedWidth}x{RequestedHeight}",
        transformedInfo.FullName,
        image.Width,
        image.Height,
        options.Width,
        options.Height
    );
}

private void TransformImage(MagickImage image, FileInfo transformedInfo, MediaTransformOptions options)
{
    using var activity = ActivitySource.StartActivity();
    image.FilterType = options is { Width: < 150 } or { Height: < 150 } ? FilterType.Box : FilterType.CubicSpline;

    logger.LogTrace(
        "Resizing Image {TransformPath} to max size {Width}x{Height}",
        transformedInfo.FullName,
        options.Width,
        options.Height
    );
    image.Scale(uint.Max(50, options.Width), uint.Max(50, options.Height));
    image.Format = options.Format.TransformFormatToMagickFormat();
    image.Depth = 24;
    image.ColorSpace = ColorSpace.Rec709YCbCr;
    image.ColorType = ColorType.TrueColor;
    image.Quality = 95;
    image.Enhance();
    image.SetAttribute("hdr:write-gain-map", true);
    image.SetProfile(ColorProfile.ColorMatchRGB, ColorTransformMode.HighRes);
    logger.LogTrace(
        "Resized Image {TransformPath} to size {Width}x{Height} - requested {RequestedWidth}x{RequestedHeight}",
        transformedInfo.FullName,
        image.Width,
        image.Height,
        options.Width,
        options.Height
    );
}

private async Task WriteImage(
    MagickImage image,
    Database.MediaDb.Entities.MediaEntry mediaEntry,
    FileInfo transformedInfo,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug(
        "Image has been transformed {OriginalPath} {TransformedPath}",
        mediaEntry.OriginalFile,
        transformedInfo.FullName
    );
    await image.WriteAsync(transformedInfo, cancellationToken);
}
}
