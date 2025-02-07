using System.Diagnostics;
using System.Runtime.CompilerServices;
using Grpc.Core;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Service.Media.Proto.Services;
using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public class MediaApi(ILogger<MediaApi> logger, MediaGrpcClient client, IMediaClient mediaFileClient) : IMediaApi
{
    private static ActivitySource ActivitySource => new(nameof(MediaApi));

public async IAsyncEnumerable<MediaItem> RandomMedia(
    uint count,
    [EnumeratorCancellation] CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug("RandomMedia start");

    using var response = client.RandomMedia(
        new MediaRequest { Count = count },
        new CallOptions()
            .WithDeadline(DateTimeOffset.UtcNow.AddMinutes(10).UtcDateTime)
            .WithCancellationToken(cancellationToken)
            .WithWaitForReady()
    );
    if (response is null)
    {
        logger.LogDebug("RandomMedia end - no random media items");
        yield break;
    }

    while (await response.ResponseStream.MoveNext(cancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = response.ResponseStream.Current;
        logger.LogTrace("RandomMedia progress");
        yield return TransformMedia(entry);
    }

    logger.LogDebug("RandomMedia end - processed all random media items");
}

public async IAsyncEnumerable<PaginatedMediaItem> PaginateMedia(
    int offset,
    int length,
    [EnumeratorCancellation] CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity();
    logger.LogDebug("PaginateMedia start");
    using var response = client.PaginateMedia(
        new PaginateMediaRequest { Offset = offset, Length = length },
        new CallOptions()
            .WithDeadline(DateTimeOffset.UtcNow.AddMinutes(10).UtcDateTime)
            .WithCancellationToken(cancellationToken)
            .WithWaitForReady()
    );
    if (response is null)
    {
        logger.LogDebug("PaginateMedia end - no media items");
        yield break;
    }

    while (await response.ResponseStream.MoveNext(cancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = response.ResponseStream.Current;
        logger.LogTrace("PaginateMedia progress");
        yield return TransformPaginatedMedia(entry);
    }

    logger.LogDebug("PaginateMedia end - processed all media items");
}

public async Task<MediaItem?> ToggleMedia(Guid mediaId, bool enabled, CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity("ToggleMedia", ActivityKind.Client);
    logger.LogDebug("ToggleMedia start");
    var response = await client.ToggleMediaAsync(
        new ToggleMediaRequest { Id = mediaId.ToString("D"), Enabled = enabled },
        new CallOptions()
            .WithDeadline(DateTimeOffset.UtcNow.AddMinutes(5).UtcDateTime)
            .WithCancellationToken(cancellationToken)
    );
    logger.LogDebug("ToggleMedia end");
    return response != null ? TransformMedia(response) : null;
}

public async Task<TransformMediaState> TransformMedia(
    Guid mediaId,
    uint width,
    uint height,
    bool blur,
    MediaTransformOptionsFormat format,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity("TransformMedia", ActivityKind.Client);
    var result = await client.TransformMediaAsync(
        new TransformMediaRequest
        {
            Id = mediaId.ToString("D"),
            Width = width,
            Height = height,
            Blur = blur,
            MediaFormat = format switch
            {
                MediaTransformOptionsFormat.Jpeg => TransformMediaFormat.Jpeg,
                MediaTransformOptionsFormat.JpegXl => TransformMediaFormat.JpegXl,
                MediaTransformOptionsFormat.Png => TransformMediaFormat.Png,
                MediaTransformOptionsFormat.WebP => TransformMediaFormat.WebP,
                MediaTransformOptionsFormat.Avif => TransformMediaFormat.Avif,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid media format requested")
            }
        },
        new CallOptions()
            .WithDeadline(DateTimeOffset.UtcNow.AddMinutes(5).UtcDateTime)
            .WithCancellationToken(cancellationToken)
    );
    return result?.State ?? TransformMediaState.NotFound;
}

public async Task<FileResponse> DownloadMedia(
    Guid mediaId,
    uint width,
    uint height,
    bool blur,
    MediaTransformOptionsFormat format,
    CancellationToken cancellationToken = default
)
{
    using var activity = ActivitySource.StartActivity("DownloadMedia", ActivityKind.Client);
    logger.LogDebug("Downloading media for {MediaId}", mediaId);
    var response = await mediaFileClient.Download_MediaAsync(
        mediaId,
        (int)width,
        (int)height,
        blur,
        format,
        cancellationToken
    );
    logger.LogDebug("Downloaded media for {MediaId} - {StatusCode}", mediaId, response.StatusCode);

    if (response.StatusCode != StatusCodes.Status200OK)
        logger.LogWarning("Failed to download media item {StatusCode}", response.StatusCode);

    return response;
}

private static MediaItem TransformMedia(MediaEntry entry) =>
    new()
    {
        Id = Guid.Parse(entry.Id),
        Created = DateTimeOffset.FromUnixTimeMilliseconds(entry.UtcDatetime),
        Notes = entry.Notes,
        Enabled = entry.Enabled,
        Location = new MediaItemLocation
        {
            Name = entry.Location,
            Latitude = entry.Latitude,
            Longitude = entry.Longitude
        },
        AspectRatio = entry.AspectRatio,
        Portrait = entry.Portrait,
        BaseB = entry.BaseB,
        BaseG = entry.BaseG,
        BaseR = entry.BaseR
    };

private static PaginatedMediaItem TransformPaginatedMedia(PaginateMediaResponse entry) =>
    new() { MediaItem = TransformMedia(entry.Entry), TotalPages = entry.Total };
}
