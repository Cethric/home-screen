using HomeScreen.Service.MediaClient.Generated;

namespace HomeScreen.Web.Slideshow.Server.Entities;

public static class MediaTransformOptionsFormatExtensions
{
    public static string TransformFormatToMime(this MediaTransformOptionsFormat format)
    {
        return format switch
        {
            MediaTransformOptionsFormat.Jpeg => "image/jpeg",
            MediaTransformOptionsFormat.JpegXL => "image/jxl",
            MediaTransformOptionsFormat.Png => "image/png",
            MediaTransformOptionsFormat.WebP => "image/webp",
            MediaTransformOptionsFormat.Avif => "image/avif",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid transform format provided")
        };
    }
}
