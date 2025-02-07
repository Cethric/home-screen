using System.Diagnostics;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace HomeScreen.Web.Common.Server.Endpoints;

public static class MediaTransformEndpoints
{
    private static ActivitySource ActivitySource => new(nameof(MediaEndpoints));

    public static void MapTransformEndpoints(this RouteGroupBuilder app)
    {
        var group = app
            .MapGroup("transform")
            // .WithTags("transform", "media")
            .WithName("TransformMedia")
            .WithDisplayName("Transform Media")
            .WithGroupName("MediaTransform");

        group
            .MapGet("item/{mediaId:guid:required}/{width:int:required}/{height:int:required}", TransformItem)
            .WithName(nameof(TransformItem))
            .WithDisplayName("Transform Item")
            .WithTags("item", "transform", "media");
        group
            .MapGet("line/{direction:int:required}/{size:int:required}", TransformLine)
            .WithName(nameof(TransformLine))
            .WithDisplayName("Transform Line")
            .WithTags("line", "transform", "media");
    }

    private static async Task<Results<NotFound, AcceptedAtRoute<AcceptedTransformMediaItem>>> TransformItem(
        Guid mediaId,
        uint width,
        uint height,
        bool blur,
        MediaTransformOptionsFormat format,
        IMediaApi service,
        HttpContext context,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity("TransformItem", ActivityKind.Client);
        var result = await service.TransformMedia(mediaId, width, height, blur, format, cancellationToken);
        if (result != TransformMediaState.Transformed) return TypedResults.NotFound();

        var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
        var url = linkGenerator.GetUriByRouteValues(
            context,
            nameof(MediaDownloadEndpoints.DownloadItem),
            new
            {
                mediaId,
                width,
                height,
                blur,
                format
            },
            fragment: FragmentString.Empty
        );
        if (url == null) return TypedResults.NotFound();

        return TypedResults.AcceptedAtRoute(
            new AcceptedTransformMediaItem
            {
                MediaId = mediaId,
                Width = width,
                Height = height,
                Blur = blur,
                Format = format,
                Url = url
            },
            nameof(MediaDownloadEndpoints.DownloadItem),
            new
            {
                mediaId,
                width,
                height,
                blur,
                format
            }
        );
    }

    private static async Task<Results<NotFound, AcceptedAtRoute<AcceptedTransformMediaLine>>> TransformLine(
        uint direction,
        uint size,
        HttpContext context,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity("TransformLine", ActivityKind.Client);

        var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
        var url = linkGenerator.GetUriByRouteValues(
            context,
            nameof(MediaDownloadEndpoints.DownloadLine),
            new
            {
                // mediaId,
                // width,
                // height,
                // blur,
                // format
            },
            fragment: FragmentString.Empty
        );
        if (url == null) return await Task.FromResult(TypedResults.NotFound());

        return await Task.FromResult(
            TypedResults.AcceptedAtRoute(
                new AcceptedTransformMediaLine
                {
                    // MediaId = mediaId,
                    // Width = width,
                    // Height = height,
                    // Blur = blur,
                    // Format = format,
                    // Url = url
                },
                nameof(MediaDownloadEndpoints.DownloadLine),
                new
                {
                    // mediaId,
                    // width,
                    // height,
                    // blur,
                    // format
                }
            )
        );
    }
}
