﻿using HomeScreen.Service.Media;
using HomeScreen.Service.MediaClient.Generated;
using HomeScreen.Web.Slideshow.Server.Entities;

namespace HomeScreen.Web.Slideshow.Server.Services;

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