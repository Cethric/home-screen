using HomeScreen.Service.MediaClient.Generated;

namespace HomeScreen.Web.Slideshow.Server.Services;

public class MediaDownloader(ILogger<MediaDownloader> logger, IMediaFileClient mediaFileClient) : IMediaDownloader
{
    public async Task<FileResponse> DownloadMedia(
        Guid id,
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("Downloading media for {MediaId}", id);
        var response = await mediaFileClient.DownloadMediaFileAsync(
            id,
            width,
            height,
            blur,
            format,
            cancellationToken
        );
        logger.LogInformation("Downloaded media for {MediaId} - {StatusCode}", id, response.StatusCode);
        return response;
    }
}
