using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using HomeScreen.Service.Media.Client.Generated;
using HomeScreen.Web.Common.JsonLines;
using HomeScreen.Web.Common.Server.Entities;
using HomeScreen.Web.Common.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace HomeScreen.Web.Common.Server.Endpoints;

public static class MediaEndpoints
{
    private static ActivitySource ActivitySource => new(nameof(MediaEndpoints));

    public static void RegisterMediaEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup("api/media")
            // .WithTags("media")
            .WithName("Media")
            .WithDisplayName("Media")
            .WithGroupName("Media");

        group
            .MapGet("random", RandomMedia)
            .WithName(nameof(RandomMedia))
            .WithDisplayName("Random Media")
            .WithTags("random", "media")
            .Produces<IEnumerable<MediaItem>>(
                StatusCodes.Status200OK,
                $"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}"
            )
            .WithRequestTimeout(TimeSpan.FromMinutes(2));
        group
            .MapGet("paginate", PaginateMedia)
            .WithName(nameof(PaginateMedia))
            .WithDisplayName("Paginate Media")
            .WithTags("paginate", "media")
            .Produces<IEnumerable<PaginatedMediaItem>>(
                StatusCodes.Status200OK,
                $"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}"
            )
            .WithRequestTimeout(TimeSpan.FromMinutes(2));
        group
            .MapPatch("item/{mediaId:guid:required}/toggle", ToggleMedia)
            .WithName(nameof(ToggleMedia))
            .WithDisplayName("Toggle Media")
            .WithTags("toggle", "media");

        group.MapDownloadEndpoints();
        group.MapTransformEndpoints();
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

    private static Task<JsonLines<PaginatedMediaItem>> PaginateMedia(
        int offset,
        int length,
        IMediaApi service,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySource.StartActivity("PaginateMedia", ActivityKind.Client);
        return Task.FromResult(
            CustomTypedResults.JsonLines(
                service.PaginateMedia(offset, length, cancellationToken),
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
}

internal class AcceptedTransformMediaItem
{
    public Guid MediaId { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public bool Blur { get; set; }
    public MediaTransformOptionsFormat Format { get; set; }
    public string Url { get; set; } = string.Empty;
}

internal class AcceptedTransformMediaLine
{
}
