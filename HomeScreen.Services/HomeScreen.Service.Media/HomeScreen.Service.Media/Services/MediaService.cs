using System.Diagnostics;
using Grpc.Core;
using HomeScreen.Service.Media.Entities;
using HomeScreen.Service.Media.Infrastructure.Media;

namespace HomeScreen.Service.Media.Services;

public class MediaService(ILogger<MediaService> logger, IMediaApi mediaApi) : Media.MediaBase
{
    private static ActivitySource ActivitySource => new(nameof(MediaService));

public override async Task RandomMedia(
    MediaRequest request,
    IServerStreamWriter<MediaEntry> responseStream,
    ServerCallContext context
)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("Count", request.Count.ToString());
    logger.LogInformation("Requested random media: {Count}", request.Count);
    await foreach (var mediaEntry in mediaApi.GetRandomMedia(request.Count, context.CancellationToken))
    {
        await responseStream.WriteAsync(mediaEntry, context.CancellationToken);
    }
}

public override async Task<MediaEntry> ToggleMedia(ToggleMediaRequest request, ServerCallContext context)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("Id", request.Id);
    logger.LogInformation("Requested toggle media: {Id}", request.Id);
    if (!Guid.TryParse(request.Id, out var id))
    {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The provided id is invalid."));
    }

    return await mediaApi.ToggleMedia(id, request.Enabled, context.CancellationToken);
}

public override async Task<TransformMediaResponse> TransformMedia(
    TransformMediaRequest request,
    ServerCallContext context
)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("Id", request.Id);
    activity?.AddBaggage("Width", request.Width.ToString());
    activity?.AddBaggage("Height", request.Height.ToString());
    activity?.AddBaggage("Blur", request.Blur.ToString());
    activity?.AddBaggage("MediaFormat", request.MediaFormat.ToString());
    logger.LogInformation("Requested transform media: {Id}", request.Id);
    if (!Guid.TryParse(request.Id, out var id))
    {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The provided id is invalid."));
    }

    var response = await mediaApi.TransformMedia(
        id,
        new MediaTransformOptions
        {
            Width = request.Width,
            Height = request.Height,
            Blur = request.Blur,
            Format = request.MediaFormat.TransformMediaFormatToMediaTransformOptionsFormat()
        },
        context.CancellationToken
    );
    return new TransformMediaResponse { State = TransformStateToTransformMediaState(response) };
}

public override async Task PaginateMedia(
    PaginateMediaRequest request,
    IServerStreamWriter<PaginateMediaResponse> responseStream,
    ServerCallContext context
)
{
    using var activity = ActivitySource.StartActivity();
    activity?.AddBaggage("Offset", request.Offset.ToString());
    activity?.AddBaggage("Length", request.Length.ToString());
    logger.LogInformation("Requested media pagination: {Offset}, {Length}", request.Offset, request.Length);
    var totalMedia = await mediaApi.GetTotalMedia(context.CancellationToken);
    await foreach (var mediaEntry in mediaApi.GetPaginatedMedia(
                       request.Offset,
                       request.Length,
                       context.CancellationToken
                   ))
    {
        await responseStream.WriteAsync(
            new PaginateMediaResponse { Entry = mediaEntry, Total = totalMedia },
            context.CancellationToken
        );
    }
}

private static TransformMediaState TransformStateToTransformMediaState(TransformState response)
{
    using var activity = ActivitySource.StartActivity();
    return response switch
    {
        TransformState.Transformed => TransformMediaState.Transformed,
        TransformState.NotFound => TransformMediaState.NotFound,
        _ => throw new ArgumentOutOfRangeException(nameof(response), response, "Invalid transform response")
    };
}
}
