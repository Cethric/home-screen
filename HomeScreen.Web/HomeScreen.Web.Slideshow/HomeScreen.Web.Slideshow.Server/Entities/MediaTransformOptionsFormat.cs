namespace HomeScreen.Web.Slideshow.Server.Entities;

public enum MediaTransformOptionsFormat
{
    Jpeg,
    WebP,
    Avif
}

public static class MediaTransformOptionsFormatExtensions
{
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
