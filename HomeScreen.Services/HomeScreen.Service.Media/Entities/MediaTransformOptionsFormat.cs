using ImageMagick;

namespace HomeScreen.Service.Media.Entities;

public enum MediaTransformOptionsFormat
{
    Jpeg,
    WebP,
    Avif
}

public static class MediaTransformOptionsFormatExtensions
{
    public static MagickFormat TransformFormatToMagickFormat(this MediaTransformOptionsFormat format)
    {
        return format switch
        {
            MediaTransformOptionsFormat.Jpeg => MagickFormat.Jpeg,
            MediaTransformOptionsFormat.WebP => MagickFormat.WebP,
            MediaTransformOptionsFormat.Avif => MagickFormat.Avif,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid transform format provided")
        };
    }

    public static string TransformFormatToMime(this MediaTransformOptionsFormat format)
    {
        return format switch
        {
            MediaTransformOptionsFormat.Jpeg => "image/jpeg",
            MediaTransformOptionsFormat.WebP => "image/webp",
            MediaTransformOptionsFormat.Avif => "image/avif",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid transform format provided")
        };
    }
}
