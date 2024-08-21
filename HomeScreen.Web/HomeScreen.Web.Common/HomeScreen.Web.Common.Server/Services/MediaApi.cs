using System.Runtime.CompilerServices;
using Grpc.Core;
using HomeScreen.Service.Media;
using HomeScreen.Service.MediaClient.Generated;
using HomeScreen.Service.Proto.Services;
using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public class MediaApi(ILogger<MediaApi> logger, MediaGrpcClient client, IMediaFileClient mediaFileClient) : IMediaApi
{
    public async IAsyncEnumerable<MediaItem> RandomMedia(
        uint count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("RandomMedia start");

        using var response = client.RandomMedia(
            new MediaRequest { Count = count },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(10).UtcDateTime)
                .WithCancellationToken(cancellationToken)
                .WithWaitForReady()
        );
        if (response is null)
        {
            logger.LogInformation("RandomMedia end - no random media items");
            yield break;
        }

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entry = response.ResponseStream.Current;
            logger.LogInformation("RandomMedia progress");
            yield return TransformMedia(entry);
        }

        logger.LogInformation("RandomMedia end - processed all random media items");
    }

    public async Task<MediaItem?> ToggleMedia(Guid mediaId, bool enabled, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("ToggleMedia start");
        var response = await client.ToggleMediaAsync(
            new ToggleMediaRequest { Id = mediaId.ToString("D"), Enabled = enabled },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(5).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        logger.LogInformation("ToggleMedia end");
        return response != null ? TransformMedia(response) : null;
    }

    public async Task<TransformMediaState> TransformMedia(
        Guid mediaId,
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    )
    {
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
                    MediaTransformOptionsFormat.JpegXL => TransformMediaFormat.JpegXl,
                    MediaTransformOptionsFormat.Png => TransformMediaFormat.Png,
                    MediaTransformOptionsFormat.WebP => TransformMediaFormat.WebP,
                    MediaTransformOptionsFormat.Avif => TransformMediaFormat.Avif,
                    _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid media format requested")
                }
            },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(5).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        return result?.State ?? TransformMediaState.NotFound;
    }

    public async Task<FileResponse> DownloadMedia(
        Guid mediaId,
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("Downloading media for {MediaId}", mediaId);
        var response = await mediaFileClient.DownloadMediaFileAsync(
            mediaId,
            width,
            height,
            blur,
            format,
            cancellationToken
        );
        logger.LogInformation("Downloaded media for {MediaId} - {StatusCode}", mediaId, response.StatusCode);
        
        if (response.StatusCode != StatusCodes.Status200OK)
        {
            logger.LogWarning("Failed to download media item {StatusCode}", response.StatusCode);
        }

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
                Name = entry.Location, Latitude = entry.Latitude, Longitude = entry.Longitude
            }
        };
}
