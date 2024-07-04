using Grpc.Core;
using HomeScreen.Service.Media.Infrastructure;
using HomeScreen.Service.Media.Infrastructure.Media;

namespace HomeScreen.Service.Media.Services;

public class MediaService(ILogger<MediaService> logger, IMediaApi mediaApi) : Media.MediaBase
{
    public override async Task RandomMedia(
        MediaRequest request,
        IServerStreamWriter<MediaEntry> responseStream,
        ServerCallContext context
    )
    {
        logger.LogInformation("Requested random media: {Count}", request.Count);
        await foreach (var mediaEntry in mediaApi.GetRandomMedia(request.Count, context.CancellationToken))
        {
            await responseStream.WriteAsync(mediaEntry, context.CancellationToken);
        }
    }

    public override async Task<MediaEntry> ToggleMedia(ToggleMediaRequest request, ServerCallContext context)
    {
        logger.LogInformation("Requested toggle media: {Id}", request.Id);
        if (Guid.TryParse(request.Id, out var id))
        {
            return await mediaApi.ToggleMedia(id, request.Enabled, context.CancellationToken);
        }

        throw new RpcException(new Status(StatusCode.InvalidArgument, $"The provided id is invalid."));
    }
}
