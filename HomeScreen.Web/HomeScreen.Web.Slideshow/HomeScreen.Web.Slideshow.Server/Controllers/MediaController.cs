using Grpc.Core;
using HomeScreen.Service.Media;
using HomeScreen.Service.Proto.Services;
using HomeScreen.Web.Slideshow.Server.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HomeScreen.Web.Slideshow.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MediaController(ILogger<MediaController> logger, MediaGrpcClient mediaGrpcClient) : ControllerBase
{
    [HttpGet(Name = "GetRandomMediaItems")]
    [ProducesResponseType<WeatherForecast>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<MediaItem>> GetRandomMediaItems(
        [FromQuery] uint count,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("GetRandomMediaItems start");
        var response = await mediaGrpcClient.RandomMediaAsync(
            new MediaRequest { Count = count },
            new CallOptions().WithDeadline(DateTimeOffset.UtcNow.AddMinutes(5).UtcDateTime)
                .WithCancellationToken(cancellationToken)
        );
        if (response is null || response.Items.Count == 0)
        {
            logger.LogWarning("No available media items found");
            logger.LogInformation("GetRandomMediaItems end");
            return new List<MediaItem>();
        }

        logger.LogInformation("GetRandomMediaItems end");
        return response.Items.Select(TransformMedia);
    }

    [HttpPatch(Name = "ToggleMediaItem")]
    [ProducesResponseType<WeatherForecast>(StatusCodes.Status202Accepted)]
    public async Task<MediaItem> ToggleMediaItem(
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
        return TransformMedia(response);
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
