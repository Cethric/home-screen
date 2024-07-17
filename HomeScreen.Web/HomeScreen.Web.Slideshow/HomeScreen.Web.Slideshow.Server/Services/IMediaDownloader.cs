using HomeScreen.Service.MediaClient.Generated;

namespace HomeScreen.Web.Slideshow.Server.Services;

public interface IMediaDownloader
{
    Task<FileResponse> DownloadMedia(
        Guid id,
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    );
}
