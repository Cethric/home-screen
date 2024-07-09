using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Grpc.Core;
using HomeScreen.Service.Media;
using HomeScreen.Service.Proto.Services;
using HomeScreen.Web.Slideshow.Server.Entities;
using HomeScreen.Web.Slideshow.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeScreen.Web.Slideshow.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MediaController(
    ILogger<MediaController> logger,
    MediaGrpcClient mediaGrpcClient,
    IMediaDownloader mediaDownloader
) : ControllerBase
{
    [HttpGet(Name = "GetRandomMediaItems")]
    [ProducesResponseType<MediaItem>(StatusCodes.Status200OK, "application/json")]
    public Task<JsonStreamingResult<MediaItem>> GetRandomMediaItems(
        [FromQuery] uint count,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(
            new JsonStreamingResult<MediaItem>(
                GetRandomMediaItemsStream(count, cancellationToken),
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            )
        );
    }

    private async IAsyncEnumerable<MediaItem> GetRandomMediaItemsStream(
        uint count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("GetRandomMediaItems start");

        using var response = mediaGrpcClient.RandomMedia(
            new MediaRequest { Count = count },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(10).UtcDateTime)
                .WithCancellationToken(cancellationToken)
                .WithWaitForReady()
        );
        if (response is null)
        {
            logger.LogInformation("GetRandomMediaItems end - no random media items");
            yield break;
        }

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entry = response.ResponseStream.Current;
            logger.LogInformation("GetRandomMediaItems progress");
            yield return TransformMedia(entry);
        }

        logger.LogInformation("GetRandomMediaItems end - processed all random media items");
    }

    [HttpPatch(Name = "ToggleMediaItem")]
    [ProducesResponseType<MediaItem>(StatusCodes.Status202Accepted, "application/json")]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaItem>> ToggleMediaItem(
        [FromQuery] Guid id,
        [FromQuery] bool enabled,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("GetRandomMediaItem start");
        var response = await mediaGrpcClient.ToggleMediaAsync(
            new ToggleMediaRequest { Id = id.ToString("D"), Enabled = enabled },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(5).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        logger.LogInformation("GetRandomMediaItem end");
        return response != null ? Ok(TransformMedia(response)) : NotFound();
    }

    [HttpGet(Name = "DownloadMediaItem")]
    [ProducesResponseType<FileResult>(StatusCodes.Status200OK, "application/x-octet-stream")]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound, "application/json")]
    [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult> DownloadMediaItem(
        [FromQuery] Guid id,
        [FromQuery] long width,
        [FromQuery] long height,
        [FromQuery] bool blur,
        [FromQuery] MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediaDownloader.DownloadMedia(
            id,
            width,
            height,
            blur,
            format,
            cancellationToken
        );

        if (result.StatusCode == HttpStatusCode.OK)
        {
            return File(await result.Content.ReadAsStreamAsync(cancellationToken), format.TransformFormatToMime());
        }

        logger.LogWarning(
            "Failed to download media item {StatusCode} {Reason}",
            result.StatusCode,
            await result.Content.ReadAsStringAsync(cancellationToken)
        );

        return result.StatusCode == HttpStatusCode.NotFound ? NotFound() : BadRequest();
    }


    [HttpGet(Name = "GetTransformMediaItemUrl")]
    [ProducesResponseType<string>(StatusCodes.Status202Accepted, "application/json")]
    public Task<AcceptedAtRouteResult> GetTransformMediaItemUrl(
        [FromQuery] Guid id,
        [FromQuery] long width,
        [FromQuery] long height,
        [FromQuery] bool blur,
        [FromQuery] MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(
            AcceptedAtRoute(
                "DownloadMediaItem",
                new
                {
                    id,
                    width,
                    height,
                    blur,
                    format
                },
                new
                {
                    id,
                    width,
                    height,
                    blur,
                    format
                }
            )
        );
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
