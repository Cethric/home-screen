using System.Diagnostics;
using System.Net.Mime;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Web.Common.Server.Entities;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Net.Http.Headers;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace HomeScreen.Web.Common.Server.Endpoints;

public static class MediaDownloadEndpoints
{
    private static ActivitySource ActivitySource => new(nameof(MediaEndpoints));

    public static void MapDownloadEndpoints(this RouteGroupBuilder app)
    {
        var group = app.MapGroup("download")
            // .WithTags("download", "media")
            .WithName("DownloadMedia")
            .WithDisplayName("Download Media")
            .WithGroupName("MediaDownload");
        group.MapGet("item/{mediaId:guid:required}/{width:int:required}/{height:int:required}", DownloadItem)
            .WithName(nameof(DownloadItem))
            .WithDisplayName("Download Item")
            .WithTags("item", "download", "media")
            .Produces<FileStreamHttpResult>(
                StatusCodes.Status200OK,
                MediaTransformOptionsFormat.Jpeg.TransformFormatToMime(),
                MediaTransformOptionsFormat.JpegXl.TransformFormatToMime(),
                MediaTransformOptionsFormat.Png.TransformFormatToMime(),
                MediaTransformOptionsFormat.WebP.TransformFormatToMime(),
                MediaTransformOptionsFormat.Avif.TransformFormatToMime()
            )
            .Produces<NotFound>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
        group.MapGet("line/{direction:int:required}/{size:int:required}", DownloadLine)
            .WithName(nameof(DownloadLine))
            .WithDisplayName("Download Line")
            .WithTags("line", "download", "media")
            .Produces<FileStreamHttpResult>(
                StatusCodes.Status200OK,
                MediaTransformOptionsFormat.Jpeg.TransformFormatToMime(),
                MediaTransformOptionsFormat.JpegXl.TransformFormatToMime(),
                MediaTransformOptionsFormat.Png.TransformFormatToMime(),
                MediaTransformOptionsFormat.WebP.TransformFormatToMime(),
                MediaTransformOptionsFormat.Avif.TransformFormatToMime()
            )
            .Produces<NotFound>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
    }

    internal static async Task<Results<FileStreamHttpResult, NotFound, BadRequest>> DownloadItem(
        Guid mediaId,
        uint width,
        uint height,
        bool blur,
        MediaTransformOptionsFormat format,
        IMediaApi service,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity("DownloadItem", ActivityKind.Client);
        var response = await service.DownloadMedia(
            mediaId,
            width,
            height,
            blur,
            format,
            cancellationToken
        );

        if (response.StatusCode == StatusCodes.Status200OK)
        {
            return TypedResults.Stream(
                response.Stream,
                format.TransformFormatToMime(),
                $"{mediaId:D}.{format}",
                DateTimeOffset.Now,
                new EntityTagHeaderValue($"\"{mediaId:D}\"", true),
                true
            );
        }

        return response.StatusCode == StatusCodes.Status404NotFound
            ? TypedResults.NotFound()
            : TypedResults.BadRequest();
    }

    internal static async Task<Results<FileStreamHttpResult, NotFound, BadRequest>> DownloadLine(
        uint direction,
        uint size
    )
    {
        using var activity = ActivitySource.StartActivity("DownloadLine", ActivityKind.Client);
        return TypedResults.NotFound();
    }
}
