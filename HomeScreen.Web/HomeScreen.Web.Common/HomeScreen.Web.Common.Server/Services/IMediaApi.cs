using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public interface IMediaApi
{
    IAsyncEnumerable<MediaItem> RandomMedia(uint count, CancellationToken cancellationToken = default);

    Task<MediaItem?> ToggleMedia(Guid mediaId, bool enabled, CancellationToken cancellationToken = default);

    Task<TransformMediaState> TransformMedia(
        Guid mediaId,
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    );

    Task<FileResponse> DownloadMedia(
        Guid mediaId,
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    );
}
