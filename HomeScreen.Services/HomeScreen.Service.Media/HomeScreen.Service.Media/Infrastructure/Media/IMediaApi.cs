using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaApi
{
    IAsyncEnumerable<MediaEntry> GetRandomMedia(uint count, CancellationToken cancellationToken = default);

    Task<MediaEntry> ToggleMedia(Guid mediaId, bool state, CancellationToken cancellationToken);

    Task<TransformState> TransformMedia(
        Guid mediaId,
        MediaTransformOptions options,
        CancellationToken cancellationToken
    );

    Task<(FileInfo, DateTimeOffset)?> GetTransformedMedia(
        Guid mediaId,
        MediaTransformOptions options,
        CancellationToken cancellationToken
    );

    IAsyncEnumerable<(MediaEntry, ulong)> GetPaginatedMedia(
        int requestOffset,
        int requestLength,
        CancellationToken cancellationToken
    );
}
