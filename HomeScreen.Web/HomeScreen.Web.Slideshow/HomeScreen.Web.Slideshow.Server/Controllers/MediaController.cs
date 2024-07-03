using System.Net;
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
    [ProducesResponseType<IEnumerable<MediaItem>>(StatusCodes.Status200OK, "application/json")]
    public async Task<ActionResult<IEnumerable<MediaItem>>> GetRandomMediaItems(
        [FromQuery] uint count,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("GetRandomMediaItems start");
        var response = await mediaGrpcClient.RandomMediaAsync(
            new MediaRequest { Count = count },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(10).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        if (response is null || response.Items.Count == 0)
        {
            logger.LogWarning("No available media items found");
            logger.LogInformation("GetRandomMediaItems end");
            return new List<MediaItem>();
        }

        logger.LogInformation("GetRandomMediaItems end");
        return Ok(response.Items.Select(TransformMedia));
    }

    [HttpPatch(Name = "ToggleMediaItem")]
    [ProducesResponseType<MediaItem>(StatusCodes.Status202Accepted, "application/json")]
    [ProducesResponseType<MediaItem>(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType<StreamContent>(StatusCodes.Status200OK, "application/x-octet-stream")]
    [ProducesResponseType<StreamContent>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<StreamContent>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StreamContent>> DownloadMediaItem(
        [FromQuery] Guid id,
        [FromQuery] long width,
        [FromQuery] long height,
        [FromQuery] float blur,
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

    private static MediaItem TransformMedia(MediaEntry entry) =>
        new MediaItem
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
