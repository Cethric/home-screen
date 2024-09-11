using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Web.Common.JsonLines;
using HomeScreen.Web.Common.Server.Entities;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Net.Http.Headers;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace HomeScreen.Web.Common.Server.Endpoints;

public static class MediaEndpoints
{
    private static ActivitySource ActivitySource => new(nameof(MediaEndpoints));

    public static void RegisterMediaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/media").WithTags("media").WithName("Media").WithGroupName("Media");

        group.MapGet("random", RandomMedia)
            .WithName(nameof(RandomMedia))
            .Produces<IEnumerable<MediaItem>>(
                StatusCodes.Status200OK,
                $"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}"
            )
            .WithRequestTimeout(TimeSpan.FromMinutes(2));
        group.MapPatch("{mediaId:guid:required}/toggle", ToggleMedia).WithName(nameof(ToggleMedia));
        group.MapGet("{mediaId:guid:required}/download/{width:int:required}/{height:int:required}", DownloadMedia)
            .WithName(nameof(DownloadMedia))
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
        group.MapGet("{mediaId:guid:required}/transform/{width:int:required}/{height:int:required}", TransformMedia)
            .WithName(nameof(TransformMedia));
    }

    private static Task<JsonLines<MediaItem>> RandomMedia(
        uint count,
        IMediaApi service,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity("RandomMedia", ActivityKind.Client);
        return Task.FromResult(
            CustomTypedResults.JsonLines(
                service.RandomMedia(count, cancellationToken),
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            )
        );
    }

    private static async Task<Results<Ok<MediaItem>, NotFound>> ToggleMedia(
        Guid mediaId,
        bool enabled,
        IMediaApi service,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity("ToggleMedia", ActivityKind.Client);
        var response = await service.ToggleMedia(mediaId, enabled, cancellationToken);
        return response != null ? TypedResults.Ok(response) : TypedResults.NotFound();
    }

    private static async Task<Results<FileStreamHttpResult, NotFound, BadRequest>> DownloadMedia(
        Guid mediaId,
        uint width,
        uint height,
        bool blur,
        MediaTransformOptionsFormat format,
        IMediaApi service,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity("DownloadMedia", ActivityKind.Client);
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

    private static async Task<Results<NotFound, AcceptedAtRoute<AcceptedTransformMeta>>> TransformMedia(
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
        using var activity = ActivitySource.StartActivity("TransformMedia", ActivityKind.Client);
        var result = await service.TransformMedia(
            mediaId,
            width,
            height,
            blur,
            format,
            cancellationToken
        );
        if (result == TransformMediaState.Transformed)
        {
            var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
            var url = linkGenerator.GetUriByRouteValues(
                context,
                nameof(DownloadMedia),
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
            if (url == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.AcceptedAtRoute(
                new AcceptedTransformMeta
                {
                    MediaId = mediaId,
                    Width = width,
                    Height = height,
                    Blur = blur,
                    Format = format,
                    Url = url
                },
                nameof(DownloadMedia),
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

        return TypedResults.NotFound();
    }
}

internal class AcceptedTransformMeta
{
    public Guid MediaId { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public bool Blur { get; set; }
    public MediaTransformOptionsFormat Format { get; set; }
    public string Url { get; set; } = string.Empty;
}
