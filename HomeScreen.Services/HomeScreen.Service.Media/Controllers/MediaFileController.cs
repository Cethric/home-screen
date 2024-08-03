using HomeScreen.Service.Media.Entities;
using HomeScreen.Service.Media.Infrastructure.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace HomeScreen.Service.Media.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MediaFileController(ILogger<MediaFileController> logger, IMediaApi mediaApi) : ControllerBase
{
    [HttpGet(Name = "DownloadMediaFile")]
    [ProducesResponseType<FileResult>(StatusCodes.Status200OK, "application/x-octet-stream")]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound, "application/json")]
    public async Task<ActionResult> DownloadMediaFile(
        [FromQuery] Guid mediaId,
        [FromQuery] int width,
        [FromQuery] int height,
        [FromQuery] bool blur,
        [FromQuery] MediaTransformOptionsFormat format,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Downloading media file for {MediaId:D}", mediaId);
        var result = await mediaApi.GetTransformedMedia(
            mediaId,
            new MediaTransformOptions { Width = width, Height = height, Blur = blur, Format = format },
            cancellationToken
        );
        logger.LogInformation("Downloaded media file for {MediaId:D}, {MediaFound:G}", mediaId, result is not null);
        if (result is not { } info) return NotFound();
        return File(
            info.Item1.Open(FileMode.Open, FileAccess.Read),
            format.TransformFormatToMime(),
            $"{mediaId:D}.{format}",
            info.Item2,
            new EntityTagHeaderValue($"\"{mediaId:D}\"", true),
            true
        );
    }
}
