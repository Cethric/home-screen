using System.Diagnostics;
using System.Net.Mime;
using HomeScreen.Service.Media.Entities;
using HomeScreen.Service.Media.Infrastructure.Media;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Net.Http.Headers;

namespace HomeScreen.Service.Media.Endpoints;

public static class MediaEndpoints
{
    private static ActivitySource ActivitySource => new(nameof(MediaEndpoints));

    public static void RegisterMediaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("media/download")
            .WithTags("media", "download")
            .WithName("Download Media")
            .WithGroupName("MediaDownload");
        group.MapGet("item/{mediaId:guid:required}/{width:int:required}/{height:int:required}", DownloadMedia)
            .WithName(nameof(DownloadMedia))
            .Produces<FileStreamHttpResult>(
                StatusCodes.Status200OK,
                MediaTransformOptionsFormat.Jpeg.TransformFormatToMime(),
                MediaTransformOptionsFormat.JpegXl.TransformFormatToMime(),
                MediaTransformOptionsFormat.Png.TransformFormatToMime(),
                MediaTransformOptionsFormat.WebP.TransformFormatToMime(),
                MediaTransformOptionsFormat.Avif.TransformFormatToMime()
            )
            .Produces<NotFound>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);

        group.MapGet("line/{direction:int:required}/{size:int:required}", DownloadLine)
            .WithName(nameof(DownloadLine))
            .Produces<FileStreamHttpResult>(
                StatusCodes.Status200OK,
                MediaTransformOptionsFormat.Jpeg.TransformFormatToMime(),
                MediaTransformOptionsFormat.JpegXl.TransformFormatToMime(),
                MediaTransformOptionsFormat.Png.TransformFormatToMime(),
                MediaTransformOptionsFormat.WebP.TransformFormatToMime(),
                MediaTransformOptionsFormat.Avif.TransformFormatToMime()
            )
            .Produces<NotFound>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
    }

    private static async Task<Results<FileStreamHttpResult, NotFound>> DownloadMedia(
        Guid mediaId,
        uint width,
        uint height,
        bool blur,
        MediaTransformOptionsFormat format,
        IMediaApi mediaApi,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity();
        activity?.AddBaggage("MediaId", mediaId.ToString());
        activity?.AddBaggage("Width", width.ToString());
        activity?.AddBaggage("Height", height.ToString());
        activity?.AddBaggage("Blur", blur.ToString());
        activity?.AddBaggage("Format", format.ToString());
        var result = await mediaApi.GetTransformedMedia(
            mediaId,
            new MediaTransformOptions { Width = width, Height = height, Blur = blur, Format = format },
            cancellationToken
        );
        if (result is not { } info)
        {
            activity?.AddEvent(new ActivityEvent("Media NotFound"));
            return TypedResults.NotFound();
        }

        activity?.AddEvent(new ActivityEvent("Media Found"));
        return TypedResults.File(
            info.Item1.Open(FileMode.Open, FileAccess.Read),
            format.TransformFormatToMime(),
            $"{mediaId:D}.{format}",
            info.Item2,
            new EntityTagHeaderValue($"\"{mediaId:D}\"", true),
            true
        );
    }

    private static async Task<Results<FileStreamHttpResult, NotFound>> DownloadLine(
        uint direction,
        uint size,
        bool blur,
        IEnumerable<string> mediaIds,
        IMediaApi mediaApi,
        CancellationToken cancellationToken
    )
    {
        return await Task.FromResult(TypedResults.NotFound());
    }
}