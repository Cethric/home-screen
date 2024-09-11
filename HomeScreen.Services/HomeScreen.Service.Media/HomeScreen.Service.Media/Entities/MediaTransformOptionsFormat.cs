using System.Diagnostics;
using System.Net.Mime;
using ImageMagick;

namespace HomeScreen.Service.Media.Entities;

public enum MediaTransformOptionsFormat
{
    Jpeg,
    JpegXl,
    Png,
    WebP,
    Avif
}

public static class MediaTransformOptionsFormatExtensions
{
    private static ActivitySource ActivitySource => new(nameof(MediaTransformOptionsFormatExtensions));

    public static MagickFormat TransformFormatToMagickFormat(this MediaTransformOptionsFormat format)
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddBaggage("Format", format.ToString());
        return format switch
        {
            MediaTransformOptionsFormat.Jpeg => MagickFormat.Jpeg,
            MediaTransformOptionsFormat.JpegXl => MagickFormat.Jxl,
            MediaTransformOptionsFormat.Png => MagickFormat.Png,
            MediaTransformOptionsFormat.WebP => MagickFormat.WebP,
            MediaTransformOptionsFormat.Avif => MagickFormat.Avif,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid transform format provided")
        };
    }

    public static string TransformFormatToMime(this MediaTransformOptionsFormat format)
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddBaggage("Format", format.ToString());
        return format switch
        {
            MediaTransformOptionsFormat.Jpeg => MediaTypeNames.Image.Jpeg,
            MediaTransformOptionsFormat.JpegXl => "image/jxl",
            MediaTransformOptionsFormat.Png => MediaTypeNames.Image.Png,
            MediaTransformOptionsFormat.WebP => MediaTypeNames.Image.Webp,
            MediaTransformOptionsFormat.Avif => MediaTypeNames.Image.Avif,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid transform format provided")
        };
    }

    public static MediaTransformOptionsFormat TransformMediaFormatToMediaTransformOptionsFormat(
        this TransformMediaFormat format
    )
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddBaggage("Format", format.ToString());
        return format switch
        {
            TransformMediaFormat.Jpeg => MediaTransformOptionsFormat.Jpeg,
            TransformMediaFormat.JpegXl => MediaTransformOptionsFormat.JpegXl,
            TransformMediaFormat.Png => MediaTransformOptionsFormat.Png,
            TransformMediaFormat.WebP => MediaTransformOptionsFormat.WebP,
            TransformMediaFormat.Avif => MediaTransformOptionsFormat.Avif,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid media format requested")
        };
    }
}
