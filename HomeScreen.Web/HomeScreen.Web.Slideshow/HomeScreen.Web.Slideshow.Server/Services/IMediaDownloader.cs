using HomeScreen.Web.Slideshow.Server.Entities;

namespace HomeScreen.Web.Slideshow.Server.Services;

public interface IMediaDownloader
{
    Task<HttpResponseMessage> DownloadMedia(
        Guid id,
        long width,
        long height,
        float blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    );
}
