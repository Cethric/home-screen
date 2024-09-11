using System.Net.Mime;
using HomeScreen.Service.Media.Client.Generated;

namespace HomeScreen.Web.Common.Server.Entities;

public static class MediaTransformOptionsFormatExtensions
{
    public static string TransformFormatToMime(this MediaTransformOptionsFormat format)
    {
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
}
