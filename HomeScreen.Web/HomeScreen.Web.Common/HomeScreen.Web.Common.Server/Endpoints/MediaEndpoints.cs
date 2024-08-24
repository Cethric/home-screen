using System.Text.Json;
using HomeScreen.Service.Media;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Web.Common.Server.Entities;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Net.Http.Headers;

namespace HomeScreen.Web.Common.Server.Endpoints;

public static class MediaEndpoints
{
    public static void RegisterMediaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/media").WithTags("media").WithName("Media").WithGroupName("Media");

        group.MapGet("random", RandomMedia).WithName(nameof(RandomMedia)).WithRequestTimeout(TimeSpan.FromMinutes(2));
        group.MapPatch("{mediaId:guid:required}/toggle", ToggleMedia).WithName(nameof(ToggleMedia));
        group.MapGet("{mediaId:guid:required}/download/{width:int:required}/{height:int:required}", DownloadMedia)
             .WithName(nameof(DownloadMedia))
             .WithRequestTimeout(TimeSpan.FromMinutes(1));
        group.MapGet("{mediaId:guid:required}/transform/{width:int:required}/{height:int:required}", TransformMedia)
             .WithName(nameof(TransformMedia));
    }

    private static Task<JsonStreamingResult<MediaItem>> RandomMedia(
        uint count,
        IMediaApi service,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult(
            CustomTypedResults.JsonStreaming(
                service.RandomMedia(count, cancellationToken),
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            )
        );

    private static async Task<Results<Ok<MediaItem>, NotFound>> ToggleMedia(
        Guid mediaId,
        bool enabled,
        IMediaApi service,
        CancellationToken cancellationToken
    )
    {
        var response = await service.ToggleMedia(mediaId, enabled, cancellationToken);
        return response != null ? TypedResults.Ok(response) : TypedResults.NotFound();
    }

    private static async Task<Results<FileStreamHttpResult, NotFound, BadRequest>> DownloadMedia(
        Guid mediaId,
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        IMediaApi service,
        CancellationToken cancellationToken
    )
    {
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
        int width,
        int height,
        bool blur,
        MediaTransformOptionsFormat format,
        IMediaApi service,
        HttpContext context,
        CancellationToken cancellationToken
    )
    {
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
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Blur { get; set; }
    public MediaTransformOptionsFormat Format { get; set; }
    public string Url { get; set; } = string.Empty;
}
