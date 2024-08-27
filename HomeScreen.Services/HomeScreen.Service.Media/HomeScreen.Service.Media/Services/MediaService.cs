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
    using var activity = ActivitySource.StartActivity("RandomMedia", ActivityKind.Client);
    logger.LogInformation("Requested random media: {Count}", request.Count);
    await foreach (var mediaEntry in mediaApi.GetRandomMedia(request.Count, context.CancellationToken))
    {
        await responseStream.WriteAsync(mediaEntry, context.CancellationToken);
    }
}

public override async Task<MediaEntry> ToggleMedia(ToggleMediaRequest request, ServerCallContext context)
{
    using var activity = ActivitySource.StartActivity("ToggleMedia", ActivityKind.Client);
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
    using var activity = ActivitySource.StartActivity("TransformMedia", ActivityKind.Client);
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
            Format = TransformMediaFormatToMediaTransformOptionsFormat(request.MediaFormat)
        },
        context.CancellationToken
    );
    return new TransformMediaResponse { State = TransformStateToTransformMediaState(response) };
}

private static MediaTransformOptionsFormat TransformMediaFormatToMediaTransformOptionsFormat(
    TransformMediaFormat format
)
{
    using var activity = ActivitySource.StartActivity(
        "TransformMediaFormatToMediaTransformOptionsFormat",
        ActivityKind.Client
    );
    return format switch
    {
        TransformMediaFormat.Jpeg => MediaTransformOptionsFormat.Jpeg,
        TransformMediaFormat.JpegXl => MediaTransformOptionsFormat.JpegXl,
        TransformMediaFormat.Png => MediaTransformOptionsFormat.Png,
        TransformMediaFormat.WebP => MediaTransformOptionsFormat.WebP,
        TransformMediaFormat.Avif => MediaTransformOptionsFormat.Avif,
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid media format requested")
    };
}

private static TransformMediaState TransformStateToTransformMediaState(TransformState response)
{
    using var activity = ActivitySource.StartActivity("TransformStateToTransformMediaState", ActivityKind.Client);
    return response switch
    {
        TransformState.Transformed => TransformMediaState.Transformed,
        TransformState.NotFound => TransformMediaState.NotFound,
        _ => throw new ArgumentOutOfRangeException(nameof(response), response, "Invalid transform response")
    };
}
}
