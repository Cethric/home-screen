using HomeScreen.Service.Media.Entities;
using HomeScreen.Service.Media.Infrastructure.Media;
using Microsoft.AspNetCore.Mvc;

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
        return result is null
            ? NotFound()
            : File(result.Open(FileMode.Open, FileAccess.Read), format.TransformFormatToMime());
    }
}
